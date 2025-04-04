using UnityEngine;

namespace PantheonQuestProgressTrackerMod.Models
{
    internal class PlayerModQuest
    {
        #region Public Properties

        public bool AlwaysTrackNewQuests { get; set; } = true;
        public long CharacterId { get; set; }
        public bool LockPosition { get; set; } = false;
        public List<ModQuest> ModQuests { get; set; } = new();
        public bool SolidBackground { get; set; }
        public float SolidBackgroundOpacity { get; set; } = 100f;
        public bool StartFresh { get; set; } = true;

        public Vector2 TrackerAnchoredPosition { get; set; } = new(0, 0);
        public float TrackerScale { get; set; } = 100f;
        public Vector2 TrackerSizeDelta { get; set; } = new(300, 300);
        public bool QuestsPanelCollapsed;

        #endregion Public Properties
    }
}