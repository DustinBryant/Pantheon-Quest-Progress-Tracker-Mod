using HarmonyLib;
using Il2Cpp;

namespace PantheonQuestProgressTrackerMod.Patches
{
    /// <summary>
    /// We need to make sure to remove our quest event handlers when the player gets destroyed
    /// </summary>
    [HarmonyPatch(typeof(EntityPlayerGameObject), nameof(EntityPlayerGameObject.NetworkStop))]
    internal class PlayerEnd
    {
        #region Private Methods

        private static void Prefix(EntityPlayerGameObject __instance)
        {
            if (__instance.NetworkId.Value != EntityPlayerGameObject.LocalPlayerId.Value
                || Global.PlayerQuests == null)
                return;

            Global.PlayerQuests = null;
        }

        #endregion Private Methods
    }
}