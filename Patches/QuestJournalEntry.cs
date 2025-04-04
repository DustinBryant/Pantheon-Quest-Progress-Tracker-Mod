using HarmonyLib;
using Il2Cpp;
using PantheonQuestProgressTrackerMod.Components;

namespace PantheonQuestProgressTrackerMod.Patches
{
    [HarmonyPatch(typeof(UIQuestJournalEntry), nameof(UIQuestJournalEntry.Awake))]
    internal class QuestJournalEntry
    {
        #region Private Methods

        private static void Postfix(UIQuestJournalEntry __instance)
        {
            __instance.gameObject.AddComponent<QuestLoadWaiter>();
        }

        #endregion Private Methods
    }
}