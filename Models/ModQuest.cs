namespace PantheonQuestProgressTrackerMod.Models
{
    internal class ModQuest
    {
        #region Public Properties

        public string GiverName { get; set; } = null!;

        public bool Collapsed { get; set; }

        public List<int> TrackedQuestIds { get; set; } = new();

        #endregion Public Properties
    }
}