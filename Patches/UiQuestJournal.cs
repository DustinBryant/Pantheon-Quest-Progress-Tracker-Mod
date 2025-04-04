using HarmonyLib;
using Il2Cpp;
using PantheonQuestProgressTrackerMod.Components;
using UnityEngine;

namespace PantheonQuestProgressTrackerMod.Patches
{
    [HarmonyPatch(typeof(UIQuestJournal), nameof(UIQuestJournal.Awake))]
    internal class UiQuestJournal
    {
        #region Private Fields

        private static UIQuestJournal? _questJournal;

        #endregion Private Fields

        #region Public Methods

        public static void RecheckCheckmarks()
        {
            if (_questJournal == null)
                return;

            foreach (var componentInChild in _questJournal.GetComponentsInChildren<UiJournalCheckBox>())
                componentInChild.RecheckIfChecked();
        }

        #endregion Public Methods

        #region Private Methods

        private static void Postfix(UIQuestJournal __instance)
        {
            _questJournal = __instance;

            var scrollView = _questJournal.transform.Find("LeftPanel").Find("ScrollView");
            var scrollViewRect = scrollView.GetComponent<RectTransform>();
            scrollViewRect.sizeDelta = new Vector2(200, 0);

            var viewPort = scrollView.Find("Viewport");
            var viewPortRect = viewPort.GetComponent<RectTransform>();
            viewPortRect.anchoredPosition = new Vector2(-17, 0);

            var scrollBar = scrollView.Find("Scrollbar");
            var scrollBarRect = scrollBar.GetComponent<RectTransform>();
            scrollBarRect.anchoredPosition = new Vector2(-4.5f, -15);
            scrollBarRect.sizeDelta = new Vector2(20, -30);

            var content = viewPort.Find("Content");
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(-72, contentRect.sizeDelta.y);
        }

        #endregion Private Methods
    }
}