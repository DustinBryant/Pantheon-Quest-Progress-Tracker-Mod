using MelonLoader;
using UnityEngine;

namespace PantheonQuestProgressTrackerMod.Components
{
    [RegisterTypeInIl2Cpp]
    internal class QuestInfoHolder : MonoBehaviour
    {
        #region Public Fields

        public int QuestId;
        public string QuestName = "";

        #endregion Public Fields
    }
}