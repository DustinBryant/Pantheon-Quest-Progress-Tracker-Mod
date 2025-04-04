using HarmonyLib;
using Il2Cpp;
using PantheonQuestProgressTrackerMod.Models;
using PantheonQuestProgressTrackerMod.Panels;

namespace PantheonQuestProgressTrackerMod.Patches
{
    [HarmonyPatch(typeof(PlayerQuests.Logic), nameof(PlayerQuests.Logic.OnAbandonQuest))]
    internal class QuestAbandoned
    {
        #region Public Methods

        public static void Prefix(PlayerQuests.Logic __instance, int questId)
        {
            TrackerPanel.RemoveQuest(questId);
        }

        #endregion Public Methods
    }

    [HarmonyPatch(typeof(PlayerQuests.Logic), nameof(PlayerQuests.Logic.OnAcceptQuest))]
    internal class QuestAccepted
    {
        #region Public Methods

        public static void Prefix(PlayerQuests.Logic __instance, int questId)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            if (characterQuests.AlwaysTrackNewQuests)
                TrackerPanel.AddQuest(questId);
        }

        #endregion Public Methods
    }

    [HarmonyPatch(typeof(PlayerQuests.Logic), nameof(PlayerQuests.Logic.OnCompleteQuest))]
    internal class QuestCompleted
    {
        #region Public Methods

        public static void Prefix(PlayerQuests.Logic __instance, int questId)
        {
            TrackerPanel.RemoveQuest(questId);
        }

        #endregion Public Methods
    }

    [HarmonyPatch(typeof(PlayerQuests.Logic), nameof(PlayerQuests.Logic.OnQuestProgressChanged))]
    internal class QuestProgressChanged
    {
        #region Public Methods

        public static void Prefix(PlayerQuests.Logic __instance, int questId, int taskIndex, int progress)
        {
            TrackerPanel.QuestProgressChanged(questId);
        }

        #endregion Public Methods
    }
}