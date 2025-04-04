using MelonLoader;
using PantheonQuestProgressTrackerMod.Models;
using UnityEngine;

namespace PantheonQuestProgressTrackerMod.Components
{
    [RegisterTypeInIl2Cpp]
    internal class TrackerPositionSaver : MonoBehaviour
    {
        #region Private Methods

        private void OnDestroy()
        {
            var rectTransform = GetComponent<RectTransform>();

            if (rectTransform == null || Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            characterQuests.TrackerAnchoredPosition = rectTransform.anchoredPosition;

            if (!characterQuests.QuestsPanelCollapsed)
                characterQuests.TrackerSizeDelta = rectTransform.sizeDelta;

            Global.TrackerModCategory.SaveToFile(false);
        }

        #endregion Private Methods
    }
}