using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace PantheonQuestProgressTrackerMod.Components
{
    [RegisterTypeInIl2Cpp]
    internal class QuestLoadWaiter : MonoBehaviour
    {
        #region Private Fields

        private bool _isQuestLoaded;
        private UIQuestJournalEntry? _questJournalEntry;
        private UiJournalCheckBox? _uiJournalCheckBox;

        #endregion Private Fields

        #region Private Methods

        private void Awake()
        {
            _questJournalEntry = gameObject.GetComponent<UIQuestJournalEntry>();

            var questTrackerCheckBox = new GameObject("QuestTrackerCheckbox");
            questTrackerCheckBox.transform.SetParent(_questJournalEntry.transform);
            _uiJournalCheckBox = questTrackerCheckBox.AddComponent<UiJournalCheckBox>();
        }

        private void Update()
        {
            if (_isQuestLoaded || _questJournalEntry == null || _uiJournalCheckBox == null || _questJournalEntry.questId == 0)
                return;

            _isQuestLoaded = true;

            _uiJournalCheckBox.Initialize(_questJournalEntry.questId);
        }

        #endregion Private Methods
    }
}