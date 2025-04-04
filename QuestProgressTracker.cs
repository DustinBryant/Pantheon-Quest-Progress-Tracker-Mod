using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;

namespace PantheonQuestProgressTrackerMod
{
    [HarmonyPatch(typeof(UIQuestJournal), nameof(UIQuestJournal.Awake))]
    internal class QuestProgressTracker
    {
        #region Private Methods

        private static void Postfix(UIQuestJournal __instance)
        {
            var fontResource = Resources
                .FindObjectsOfTypeAll<Font>()
                .FirstOrDefault(resourceFont => resourceFont.name.Equals("Roboto-Bold", StringComparison.OrdinalIgnoreCase));
            var robotoBold = TMP_FontAsset.CreateFontAsset(fontResource);

            var trackingHeader = new GameObject("TrackingJournalHeader");
            var journalBackground = __instance.transform.Find("Background");
            trackingHeader.transform.SetParent(journalBackground);
            var trackingHeaderRect = trackingHeader.AddComponent<RectTransform>();
            trackingHeaderRect.anchorMax = new Vector2(1, 1);
            trackingHeaderRect.anchorMin = new Vector2(0, 1);
            trackingHeaderRect.offsetMax = new Vector2(0, 0);
            trackingHeaderRect.offsetMin = new Vector2(0, 0);
            trackingHeaderRect.anchoredPosition = new Vector2(121, -86);
            trackingHeaderRect.localScale = Vector3.one;
            var headerText = trackingHeader.AddComponent<TextMeshProUGUI>();
            headerText.text = "Track";
            headerText.fontSize = 11;
            headerText.fontStyle = FontStyles.Normal | FontStyles.Bold | FontStyles.Underline;
            headerText.color = new Color(0, 0, 0, 1);
            headerText.font = robotoBold;
        }
    }

    #endregion Private Methods
}