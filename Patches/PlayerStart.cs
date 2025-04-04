using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using MelonLoader.Utils;
using PantheonQuestProgressTrackerMod.Models;
using PantheonQuestProgressTrackerMod.Panels;
using UnityEngine.SceneManagement;

namespace PantheonQuestProgressTrackerMod.Patches
{
    /// <summary>
    /// This gets us the player object and the players quests
    /// </summary>
    [HarmonyPatch(typeof(EntityPlayerGameObject), nameof(EntityPlayerGameObject.NetworkStart))]
    internal class PlayerStart
    {
        #region Private Methods

        private static void LoadAllQuests()
        {
            if (Global.PlayerQuests == null || Global.TrackerModCategory == null)
                return;

            var savedTrackedQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            savedTrackedQuests.ModQuests.Clear();
            Global.TrackerModCategory.SaveToFile(false);

            var activeQuests = Global.PlayerQuests.GetAllActiveClientQuests();
            foreach (var i in activeQuests)
            {
                var questGiver = savedTrackedQuests.ModQuests
                    .FirstOrDefault(q => q.GiverName == i.Value.GiverName);

                if (questGiver == null)
                {
                    savedTrackedQuests.ModQuests.Add(
                        new ModQuest { GiverName = i.Value.GiverName, TrackedQuestIds = new List<int> { i.Value.QuestId }, Collapsed = false });
                    continue;
                }

                questGiver.TrackedQuestIds.Add(i.Value.QuestId);
            }

            Global.TrackerModCategory.SaveToFile(false);
            Global.TrackedQuests = savedTrackedQuests.ModQuests;
        }

        private static void LoadTrackedQuests()
        {
            if (Global.PlayerQuests == null || Global.TrackerModCategory == null)
                return;

            var playerModQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var activeQuests = Global.PlayerQuests.GetAllActiveClientQuests();

            if (activeQuests == null || playerModQuests.ModQuests.Count < 1)
                return;

            // Remove any bad quests
            var badQuestIds = new List<int>();
            foreach (var questId in playerModQuests.ModQuests.SelectMany(quests => quests.TrackedQuestIds))
            {
                var found = false;
                foreach (var activeQuest in activeQuests)
                {
                    if (questId != activeQuest.Value.QuestId)
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    badQuestIds.Add(questId);
            }

            foreach (var questId in badQuestIds)
            {
                var questMod = playerModQuests.ModQuests
                    .First(q => q.TrackedQuestIds
                    .Any(tq => tq == questId));
                questMod.TrackedQuestIds = questMod.TrackedQuestIds
                    .Where(tq => tq != questId)
                    .ToList();
            }

            // Remove any givers without quest now that we removed bad quests
            playerModQuests.ModQuests = playerModQuests.ModQuests
                .Where(q => q.TrackedQuestIds.Count > 0)
                .ToList();

            Global.TrackerModCategory.SaveToFile(false);
            Global.TrackedQuests = playerModQuests.ModQuests;
        }

        private static void Postfix(EntityPlayerGameObject __instance)
        {
            if (__instance.NetworkId.Value != EntityPlayerGameObject.LocalPlayerId.Value)
                return;

            var playerQuests = __instance.GetComponent<PlayerQuests>();

            if (playerQuests == null)
                return;

            // Set up preferences
            Global.CharacterId = __instance.Info.CharacterId;
            Global.PlayerName = __instance.info.DisplayName;
            Global.PlayerQuests = playerQuests.logic;

            Global.TrackerModCategory = MelonPreferences.CreateCategory($"TrackerModCategory{Global.CharacterId}", "Quest Progress Tracker");
            Global.TrackerModCategory.SetFilePath(Path.Combine(Path.GetDirectoryName(MelonEnvironment.UserDataDirectory) ?? "", "UserData", "QuestProgressTrackerMod.cfg"));
            if (!Global.TrackerModCategory.HasEntry(Global.TrackerModCategoryPlayersQuests))
            {
                Global.TrackerModCategory.CreateEntry(Global.TrackerModCategoryPlayersQuests, new PlayerModQuest());
                Global.TrackerModCategory.SaveToFile(false);
            }

            var playersModQuestEntry = Global.TrackerModCategory.GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;
            playersModQuestEntry.CharacterId = Global.CharacterId;
            Global.TrackerModCategory.SaveToFile(false);

            Global.SolidBackground = playersModQuestEntry.SolidBackground;

            if (playersModQuestEntry.StartFresh)
            {
                LoadAllQuests();
                playersModQuestEntry.StartFresh = false;
                Global.TrackerModCategory.SaveToFile(false);
            }
            else
                LoadTrackedQuests();

            var midPanel = SceneManager.GetActiveScene()
                .GetRootGameObjects()
                .First(obj => obj.name.Equals("HudCanvas(Clone)", StringComparison.OrdinalIgnoreCase)).transform
                .Find("Panel_Mid");

            TrackerPanel.CreateTrackerPanel(midPanel);
            TrackerPanelSettings.CreateSettingsPanel(midPanel);
            TrackerPanel.LoadTrackerContent();
        }

        #endregion Private Methods
    }
}