using Il2Cpp;
using MelonLoader;
using MelonLoader.Utils;
using PantheonQuestProgressTrackerMod.Models;
using UnityEngine;
using UnityEngine.Bindings;

namespace PantheonQuestProgressTrackerMod
{
    internal static class Global
    {
        #region Public Fields

        public static long CharacterId = 0;
        public static bool IsResizing = false;
        public static string PlayerName = "";
        public static PlayerQuests.Logic? PlayerQuests;
        public static GameObject? QuestTrackerGameObject;
        public static bool SolidBackground = false;
        public static List<ModQuest> TrackedQuests = new();
        public static MelonPreferences_Category? TrackerModCategory = null!;
        public static string TrackerModCategoryPlayersQuests = "PlayersQuests";

        #endregion Public Fields

        #region Public Methods

        public static ClientQuest? GetClientQuest(int questId)
        {
            if (PlayerQuests == null)
                return new ClientQuest();

            var activeQuests = PlayerQuests.GetAllActiveClientQuests();

            return activeQuests[questId];
        }

        public static bool IsQuestTracked(int questId)
        {
            return TrackedQuests.Any(q => q.TrackedQuestIds.Any(tq => tq == questId));
        }

        /// <summary>
        /// Uses the path of the mod directory + provided imageName for our mod to load images from into Texture2D objects.
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static Texture2D LoadImageToTexture2d(string imageName)
        {
            var imageLocation = Path.Combine(Path.GetDirectoryName(MelonEnvironment.ModsDirectory)!, "Mods", "pantheon-quest-progress-tracker-mod", imageName);
            var imageAsBytes = File.ReadAllBytes(imageLocation);
            var image = new Texture2D(2, 2);

            unsafe
            {
                var intPtr = UnityEngine.Object.MarshalledUnityObject.MarshalNotNull(image);

                fixed (byte* ptr = imageAsBytes)
                {
                    var managedSpanWrapper = new ManagedSpanWrapper(ptr, imageAsBytes.Length);

                    ImageConversion.LoadImage_Injected(intPtr, ref managedSpanWrapper, false);
                }
            }

            return image;
        }

        #endregion Public Methods
    }
}