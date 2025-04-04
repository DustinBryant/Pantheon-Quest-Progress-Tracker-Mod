using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using PantheonQuestProgressTrackerMod.Components;
using PantheonQuestProgressTrackerMod.Models;
using PantheonQuestProgressTrackerMod.Patches;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PantheonQuestProgressTrackerMod.Panels
{
    internal static class TrackerPanel
    {
        #region Private Fields

        private static RectTransform? _questsWrapperRect;
        private static UIDraggable? _uiDraggable;
        private static UIWindowPanel? _uiWindowPanel;

        #endregion Private Fields

        #region Public Methods

        public static void AddQuest(int questId)
        {
            var clientQuest = Global.GetClientQuest(questId);

            if (clientQuest == null)
                return;

            var existingQuestGiver = Global.TrackedQuests
                .FirstOrDefault(giver => giver.GiverName.Equals(clientQuest.GiverName, StringComparison.OrdinalIgnoreCase));

            if (existingQuestGiver != null)
            {
                existingQuestGiver.TrackedQuestIds.Add(questId);
                Global.TrackerModCategory?.SaveToFile(false);

                if (_questsWrapperRect == null)
                    return;

                var questGiver = _questsWrapperRect
                    .GetComponentsInChildren<GiverNameHolder>()
                    .FirstOrDefault(giver => giver.GiverName.Equals(clientQuest.GiverName, StringComparison.OrdinalIgnoreCase));

                if (questGiver == null)
                    return;

                var quest = CreateQuest(clientQuest);

                // Loop through each quest in the quest giver to find where to insert our new quest alphabetically
                var allChildQuests = questGiver.gameObject.transform
                    .GetChild(1)
                    .GetComponentsInChildren<QuestInfoHolder>();

                quest.transform.SetParent(questGiver.transform.GetChild(1));

                for (var i = 0; i < allChildQuests.Length; i++)
                {
                    var childQuest = allChildQuests[i];
                    if (string.CompareOrdinal(clientQuest.Name, childQuest.QuestName) >= 0)
                        continue;

                    quest.transform.SetSiblingIndex(i);
                    break;
                }

                quest.GetComponent<RectTransform>().localScale = Vector3.one;

                // Force layout
                var questGiverGroup = questGiver.gameObject;
                var layoutElement = questGiverGroup.GetComponent<LayoutElement>();

                if (layoutElement)
                    Object.Destroy(layoutElement);

                LayoutRebuilder.ForceRebuildLayoutImmediate(_questsWrapperRect);

                MelonCoroutines.Start(ResizeQuestGiverGroup(questGiverGroup));
            }
            else
            {
                var modQuest = new ModQuest { GiverName = clientQuest.GiverName };
                modQuest.TrackedQuestIds.Add(questId);
                Global.TrackedQuests.Add(modQuest);
                Global.TrackerModCategory?.SaveToFile(false);

                if (_questsWrapperRect == null)
                    return;

                var questGiverGroup = CreateQuestGiver(clientQuest.GiverName);

                // Loop through each quest giver to find where to insert our new quest giver alphabetically
                var allChildQuestGivers = _questsWrapperRect
                    .GetComponentsInChildren<GiverNameHolder>();

                questGiverGroup.transform.SetParent(_questsWrapperRect.gameObject.transform);
                var quest = CreateQuest(clientQuest);
                quest.transform.SetParent(questGiverGroup.transform.GetChild(1));
                quest.GetComponent<RectTransform>().localScale = Vector3.one;

                for (var i = 0; i < allChildQuestGivers.Length; i++)
                {
                    var childQuestGiver = allChildQuestGivers[i];

                    if (string.CompareOrdinal(clientQuest.GiverName, childQuestGiver.GiverName) >= 0)
                        continue;

                    questGiverGroup.transform.SetSiblingIndex(i);
                    break;
                }

                questGiverGroup.GetComponent<RectTransform>().localScale = Vector3.one;
                MelonCoroutines.Start(ResizeQuestGiverGroup(questGiverGroup));

                _questsWrapperRect.Find("Flexible Element")?.SetAsLastSibling();
            }

            MelonCoroutines.Start(RebuildLayoutDelayed());
        }

        public static void CreateTrackerPanel(Transform midPanel)
        {
            if (_questsWrapperRect != null || Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            // Quest Progress Tracker panel
            Global.QuestTrackerGameObject = new GameObject("Panel_QuestProgressTracker");
            Global.QuestTrackerGameObject.transform.SetParent(midPanel);
            Global.QuestTrackerGameObject.layer = Layers.UI;
            var trackerRect = Global.QuestTrackerGameObject.AddComponent<RectTransform>();
            trackerRect.anchoredPosition = characterQuests.TrackerAnchoredPosition == Vector2.zero ?
                new Vector2(Screen.width / 4f - 150, (Screen.height / 4f - 150) * -1) :
                characterQuests.TrackerAnchoredPosition;
            trackerRect.sizeDelta = characterQuests.TrackerSizeDelta;
            trackerRect.anchorMax = Vector2.up;
            trackerRect.anchorMin = Vector2.up;
            trackerRect.pivot = Vector2.up;
            var trackerScale = characterQuests.TrackerScale;
            trackerRect.localScale = new Vector3(trackerScale / 100, trackerScale / 100, trackerScale / 100);
            Global.QuestTrackerGameObject.AddComponent<MouseOverOpacityHandler>();

            // prevent mouse from clicking through the tracker panel
            var trackerPreventMouseImage = Global.QuestTrackerGameObject.AddComponent<Image>();
            trackerPreventMouseImage.color = new Color(0, 0, 0, 0);
            trackerPreventMouseImage.raycastTarget = true;
            Global.QuestTrackerGameObject.AddComponent<GraphicRaycaster>();

            // Add window panel components
            _uiDraggable = Global.QuestTrackerGameObject.AddComponent<UIDraggable>();
            _uiWindowPanel = Global.QuestTrackerGameObject.AddComponent<UIWindowPanel>();
            var canvasGroup = Global.QuestTrackerGameObject.AddComponent<CanvasGroup>();
            Global.QuestTrackerGameObject.AddComponent<TrackerPositionSaver>();
            _uiWindowPanel.CanvasGroup = canvasGroup;
            _uiWindowPanel._displayName = "";

            // Background object of the tracker panel
            var trackerPanelBackground = new GameObject("Background");
            trackerPanelBackground.transform.SetParent(Global.QuestTrackerGameObject.transform);
            var backgroundRect = trackerPanelBackground.AddComponent<RectTransform>();
            backgroundRect.anchoredPosition = new Vector2(1.5f, -1.5f);
            backgroundRect.anchorMax = new Vector2(1f, 1f);
            backgroundRect.anchorMin = new Vector2(0f, 0f);
            backgroundRect.sizeDelta = new Vector2(-1f, -1f);
            backgroundRect.localScale = Vector3.one;
            var backgroundCanvasRenderer = trackerPanelBackground.AddComponent<CanvasRenderer>();
            backgroundCanvasRenderer.cullTransparentMesh = false;
            var backgroundImage = trackerPanelBackground.AddComponent<Image>();
            var backgroundImageTexture = Global.LoadImageToTexture2d("BlackBackground.png");
            backgroundImageTexture.filterMode = FilterMode.Point;
            backgroundImage.sprite = Sprite.Create(backgroundImageTexture,
                new Rect(0, 0, backgroundImageTexture.width, backgroundImageTexture.height),
                new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight,
                new Vector4(12, 12, 12, 12));
            backgroundImage.type = Image.Type.Sliced;
            var opacityHandler = trackerPanelBackground.AddComponent<ImageOpacityHandler>();
            opacityHandler.MainBackground = true;

            // Panel Title
            var panelTitleObject = new GameObject("Title");
            panelTitleObject.transform.SetParent(trackerPanelBackground.transform);
            var titleRect = panelTitleObject.AddComponent<RectTransform>();
            titleRect.localScale = new Vector3(1, 1, 1);
            titleRect.anchoredPosition = new Vector2(8, -8);
            titleRect.anchorMax = new Vector2(0, 1);
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.pivot = new Vector2(0, 1);
            titleRect.sizeDelta = new Vector2(59, 14);
            var titleCanvasRenderer = panelTitleObject.AddComponent<CanvasRenderer>();
            titleCanvasRenderer.cullTransparentMesh = false;
            var titleImage = panelTitleObject.AddComponent<Image>();
            var titleImageTexture = Global.LoadImageToTexture2d("quests.png");
            titleImageTexture.filterMode = FilterMode.Point;
            titleImage.sprite = Sprite.Create(titleImageTexture, new Rect(0, 0, titleImageTexture.width, titleImageTexture.height), new Vector2(0.5f, 0.5f));
            var titleLayoutElement = panelTitleObject.AddComponent<LayoutElement>();
            titleLayoutElement.ignoreLayout = true;

            // Settings button
            var settingsButton = new GameObject("Settings Button");
            settingsButton.transform.SetParent(trackerPanelBackground.transform);
            var settingsButtonRect = settingsButton.AddComponent<RectTransform>();
            settingsButtonRect.anchoredPosition = new Vector2(-36, -14);
            settingsButtonRect.anchorMax = new Vector2(1, 1);
            settingsButtonRect.anchorMin = new Vector2(1, 1);
            settingsButtonRect.sizeDelta = new Vector2(14, 14);
            settingsButtonRect.localScale = new Vector3(1, 1, 1);
            var settingsButtonRenderer = settingsButton.AddComponent<CanvasRenderer>();
            settingsButtonRenderer.cullTransparentMesh = false;
            var settingsButtonImage = settingsButton.AddComponent<Image>();
            var settingsButtonTexture = Global.LoadImageToTexture2d("settings.png");
            settingsButtonTexture.filterMode = FilterMode.Point;
            settingsButtonImage.sprite = Sprite.Create(settingsButtonTexture, new Rect(0, 0, settingsButtonTexture.width, settingsButtonTexture.height), new Vector2(0.5f, 0.5f));
            settingsButton.AddComponent<ImageOpacityHandler>();
            var settingsButtonButton = settingsButton.AddComponent<Button>();
            settingsButtonButton.onClick.AddListener(new Action(TrackerPanelSettings.OpenPanelToggle));
            var settingsButtonLayoutElement = settingsButton.AddComponent<LayoutElement>();
            settingsButtonLayoutElement.ignoreLayout = true;

            // minimize/maximize button
            var minMaxButton = new GameObject("MinMax Button");
            minMaxButton.transform.SetParent(trackerPanelBackground.transform);
            var minMaxButtonRect = minMaxButton.AddComponent<RectTransform>();
            minMaxButtonRect.anchoredPosition = new Vector2(-9, -6.8f);
            minMaxButtonRect.anchorMax = Vector2.one;
            minMaxButtonRect.anchorMin = Vector2.one;
            minMaxButtonRect.pivot = Vector2.one;
            minMaxButtonRect.sizeDelta = new Vector2(14, 14);
            minMaxButtonRect.localScale = Vector3.one;
            var minMaxButtonRenderer = minMaxButton.AddComponent<CanvasRenderer>();
            minMaxButtonRenderer.cullTransparentMesh = false;
            var minMaxButtonImage = minMaxButton.AddComponent<Image>();
            var minMaxButtonTexture = Global.LoadImageToTexture2d("chev_button.png");
            minMaxButtonTexture.filterMode = FilterMode.Point;
            minMaxButtonImage.sprite = Sprite.Create(minMaxButtonTexture, new Rect(0, 0, minMaxButtonTexture.width, minMaxButtonTexture.height), new Vector2(0.5f, 0.5f));
            minMaxButton.AddComponent<ImageOpacityHandler>();
            var minMaxButtonButton = minMaxButton.AddComponent<Button>();
            minMaxButtonButton.onClick.AddListener(new Action(() => CollapseToggleQuestsPanel(true)));
            var minMaxButtonLayoutElement = minMaxButton.AddComponent<LayoutElement>();
            minMaxButtonLayoutElement.ignoreLayout = true;

            // Vertical resize bar
            var resizeBar = new GameObject("Resize Bar");
            resizeBar.transform.SetParent(trackerPanelBackground.transform);
            var resizeBarRect = resizeBar.AddComponent<RectTransform>();
            resizeBarRect.anchoredPosition = new Vector2(0, 0);
            resizeBarRect.anchorMax = new Vector2(1, 0);
            resizeBarRect.anchorMin = new Vector2(0, 0);
            resizeBarRect.pivot = new Vector2(0, 0);
            resizeBarRect.localScale = new Vector3(1, 1, 1);
            resizeBarRect.sizeDelta = new Vector2(0, 10);
            var resizeHandler = resizeBar.AddComponent<WindowResizeHandle>();
            resizeHandler.ResizeWindowRect = trackerRect;
            resizeHandler.Draggable = _uiDraggable;
            var resizeBarBackgroundLayoutElement = resizeBar.AddComponent<LayoutElement>();
            resizeBarBackgroundLayoutElement.ignoreLayout = true;

            var resizeBarGrabBar = new GameObject("Grab Bar");
            resizeBarGrabBar.transform.SetParent(resizeBar.transform);
            var resizeBarGrabBarRect = resizeBarGrabBar.AddComponent<RectTransform>();
            resizeBarGrabBarRect.anchoredPosition = new Vector2(0, 0);
            resizeBarGrabBarRect.sizeDelta = new Vector2(70, 2);
            resizeBarGrabBarRect.localScale = new Vector3(1, 1, 1);
            var resizeBarGrabBarImage = resizeBarGrabBar.AddComponent<Image>();
            var resizeBarGrabBarTexture = Global.LoadImageToTexture2d("GrayBar.png");
            resizeBarGrabBarTexture.filterMode = FilterMode.Point;
            resizeBarGrabBarImage.sprite = Sprite.Create(resizeBarGrabBarTexture,
                new Rect(0, 0, resizeBarGrabBarTexture.width, resizeBarGrabBarTexture.height),
                new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight,
                new Vector4(12, 7, 12, 7));
            resizeBarGrabBarImage.type = Image.Type.Sliced;
            resizeBarGrabBar.AddComponent<ImageOpacityHandler>();
            var resizeBarGrabBarLayoutElement = resizeBarGrabBar.AddComponent<LayoutElement>();
            resizeBarGrabBarLayoutElement.ignoreLayout = true;

            // Check if we need to disable uiDraggable
            var lockPosition = characterQuests.LockPosition;
            _uiDraggable.enabled = !lockPosition;
            _uiWindowPanel.Show();

            if (!Global.SolidBackground)
                return;

            var opacityImages = Global.QuestTrackerGameObject
                .GetComponentsInChildren<ImageOpacityHandler>()
                .Where(op => !op.MainBackground)
                .Select(op => op.gameObject.GetComponent<Image>());

            foreach (var image in opacityImages)
                if (image != null)
                    image.color = image.color with { a = 1 };

            backgroundImage.color = backgroundImage.color with { a = characterQuests.SolidBackgroundOpacity / 100 };
        }

        public static void LoadTrackerContent()
        {
            if (Global.QuestTrackerGameObject == null || _questsWrapperRect != null)
                return;

            var questScrollView = new GameObject("Quest Scroll View");
            questScrollView.transform.SetParent(Global.QuestTrackerGameObject.transform);
            var questViewRect = questScrollView.AddComponent<RectTransform>();
            questViewRect.anchoredPosition = new Vector2(0, 0);
            questViewRect.anchorMax = new Vector2(1, 1);
            questViewRect.anchorMin = new Vector2(0, 0);
            questViewRect.offsetMax = new Vector2(0, 0);
            questViewRect.offsetMin = new Vector2(0, 0);
            questViewRect.sizeDelta = new Vector2(0, 0);
            questViewRect.localScale = Vector3.one;
            var questViewScrollRect = questScrollView.AddComponent<ScrollRect>();
            questViewScrollRect.movementType = ScrollRect.MovementType.Clamped;
            questViewScrollRect.horizontal = false;
            questViewScrollRect.scrollSensitivity = 20;

            var viewPort = new GameObject("Quest View Port");
            viewPort.transform.SetParent(questScrollView.transform);
            var viewPortRect = viewPort.AddComponent<RectTransform>();
            viewPortRect.anchorMax = new Vector2(1, 1);
            viewPortRect.anchorMin = new Vector2(0, 0);
            viewPortRect.offsetMax = new Vector2(-22, -29);
            viewPortRect.offsetMin = new Vector2(13, 10);
            viewPortRect.localScale = new Vector3(1, 1, 1);
            var viewPortMask = viewPort.AddComponent<Mask>();
            viewPortMask.showMaskGraphic = false;
            viewPort.AddComponent<CanvasRenderer>();
            var viewPortImage = viewPort.AddComponent<Image>();
            viewPortImage.color = new Color(1, 1, 1, 1);
            questViewScrollRect.viewport = viewPortRect;

            var scrollBar = new GameObject("Quest Scrollbar");
            scrollBar.transform.SetParent(questScrollView.transform);
            var scrollBarRect = scrollBar.AddComponent<RectTransform>();
            scrollBarRect.anchoredPosition = new Vector2(-12.5f, -9.5f);
            scrollBarRect.anchorMax = Vector2.one;
            scrollBarRect.anchorMin = new Vector2(1, 0);
            scrollBarRect.pivot = new Vector2(0.5f, 0.5f);
            scrollBarRect.sizeDelta = new Vector2(20, -60);
            scrollBarRect.localScale = new Vector3(0.5f, 1, 1);
            var scrollBarImage = scrollBar.AddComponent<Image>();
            var scrollBarImageTexture = Global.LoadImageToTexture2d("UI_ScrollBar_Bkg.png");
            scrollBarImageTexture.filterMode = FilterMode.Point;
            scrollBarImage.sprite = Sprite.Create(scrollBarImageTexture,
                new Rect(0, 0, scrollBarImageTexture.width, scrollBarImageTexture.height),
                new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight,
                new Vector4(3, 3, 3, 3));
            scrollBarImage.type = Image.Type.Sliced;
            var scrollBarScrollBar = scrollBar.AddComponent<Scrollbar>();
            scrollBarScrollBar.direction = Scrollbar.Direction.BottomToTop;

            questViewScrollRect.verticalScrollbar = scrollBarScrollBar;

            var scrollBarSliderArea = new GameObject("Sliding Area");
            scrollBarSliderArea.transform.SetParent(scrollBar.transform);
            var scrollBarSliderAreaRect = scrollBarSliderArea.AddComponent<RectTransform>();
            scrollBarSliderAreaRect.anchoredPosition = Vector2.zero;
            scrollBarSliderAreaRect.anchorMax = Vector2.one;
            scrollBarSliderAreaRect.anchorMin = Vector2.zero;
            scrollBarSliderAreaRect.pivot = new Vector2(0.5f, 0.5f);
            scrollBarSliderAreaRect.sizeDelta = Vector2.zero;
            scrollBarSliderAreaRect.localScale = Vector3.one;

            var scrollBarSliderHandle = new GameObject("Handle");
            scrollBarSliderHandle.transform.SetParent(scrollBarSliderArea.transform);
            var scrollBarSliderHandleRect = scrollBarSliderHandle.AddComponent<RectTransform>();
            scrollBarSliderHandleRect.anchoredPosition = new Vector2(0.0001f, 0);
            scrollBarSliderHandleRect.anchorMax = Vector2.one;
            scrollBarSliderHandleRect.anchorMin = new Vector2(0, 0.649f);
            scrollBarSliderHandleRect.pivot = new Vector2(0.5f, 0.5f);
            scrollBarSliderHandleRect.sizeDelta = new Vector2(0.0001f, 0);
            scrollBarSliderHandleRect.localScale = Vector3.one;
            var scrollBarSliderHandleImage = scrollBarSliderHandle.AddComponent<Image>();
            var scrollBarSliderHandleImageTexture = Global.LoadImageToTexture2d("UI_ScrollBar_Thumb.png");
            scrollBarSliderHandleImageTexture.filterMode = FilterMode.Point;
            scrollBarSliderHandleImage.sprite = Sprite.Create(scrollBarSliderHandleImageTexture,
                new Rect(0, 0, scrollBarSliderHandleImageTexture.width, scrollBarSliderHandleImageTexture.height),
                new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight,
                new Vector4(1, 0, 1, 0));
            scrollBarSliderHandleImage.type = Image.Type.Sliced;

            scrollBarScrollBar.handleRect = scrollBarSliderHandleRect;

            var capTop = new GameObject("CapTop");
            capTop.transform.SetParent(scrollBar.transform);
            var capTopRect = capTop.AddComponent<RectTransform>();
            capTopRect.anchoredPosition = new Vector2(0, 5);
            capTopRect.anchorMax = new Vector2(0.5f, 1);
            capTopRect.anchorMin = new Vector2(0.5f, 1);
            capTopRect.pivot = new Vector2(0.5f, 0.5f);
            capTopRect.sizeDelta = new Vector2(19, 10.801f);
            capTopRect.localScale = Vector3.one;
            var capTopImage = capTop.AddComponent<Image>();
            var capTopTexture = Global.LoadImageToTexture2d("UI_ScrollBar_Cap.png");
            capTopTexture.filterMode = FilterMode.Point;
            capTopImage.sprite = Sprite.Create(capTopTexture, new Rect(0, 0, capTopTexture.width, capTopTexture.height), new Vector2(0.5f, 0.5f));

            var capBottom = new GameObject("CapBottom");
            capBottom.transform.SetParent(scrollBar.transform);
            var capBottomRect = capBottom.AddComponent<RectTransform>();
            capBottomRect.anchoredPosition = new Vector2(0, -5);
            capBottomRect.anchorMax = new Vector2(0.5f, 0);
            capBottomRect.anchorMin = new Vector2(0.5f, 0);
            capBottomRect.pivot = new Vector2(0.5f, 0.5f);
            capBottomRect.sizeDelta = new Vector2(19, 10.801f);
            capBottomRect.localScale = Vector3.one;
            var capBottomImage = capBottom.AddComponent<Image>();
            var capBottomTexture = Global.LoadImageToTexture2d("UI_ScrollBar_Cap.png");
            capBottomTexture.filterMode = FilterMode.Point;
            capBottomImage.sprite = Sprite.Create(capBottomTexture, new Rect(0, 0, capBottomTexture.width, capBottomTexture.height), new Vector2(0.5f, 0.5f));
            capBottomRect.Rotate(new Vector3(180, 0, 0));

            var content = new GameObject("Content");
            content.transform.SetParent(viewPort.transform);
            _questsWrapperRect = content.AddComponent<RectTransform>();
            _questsWrapperRect.anchoredPosition = Vector2.zero;
            _questsWrapperRect.anchorMax = Vector2.one;
            _questsWrapperRect.anchorMin = Vector2.up;
            _questsWrapperRect.pivot = Vector2.up;
            _questsWrapperRect.sizeDelta = new Vector2(265, 0);
            _questsWrapperRect.localScale = Vector3.one;
            questViewScrollRect.content = _questsWrapperRect;
            var contentVerticalLayoutGroup = content.AddComponent<VerticalLayoutGroup>();
            contentVerticalLayoutGroup.childForceExpandHeight = false;
            contentVerticalLayoutGroup.childScaleHeight = false;
            contentVerticalLayoutGroup.childControlWidth = false;
            contentVerticalLayoutGroup.childForceExpandWidth = false;
            contentVerticalLayoutGroup.spacing = 25;
            var contentContentSizeFitter = content.AddComponent<ContentSizeFitter>();
            contentContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (var questGiverDetails in Global.TrackedQuests
              .OrderBy(questGiverDetail => questGiverDetail.GiverName))
            {
                var questGiverGroup = CreateQuestGiver(questGiverDetails.GiverName);
                questGiverGroup.transform.SetParent(content.transform);
                questGiverGroup.GetComponent<RectTransform>().localScale = Vector3.one;
                var quests = questGiverGroup.transform.FindChild("Quests").gameObject;

                foreach (var clientQuest in questGiverDetails.TrackedQuestIds
                  .Select(Global.GetClientQuest)
                  .OfType<ClientQuest>()
                  .OrderBy(qst => qst.Name))
                {
                    var quest = CreateQuest(clientQuest);
                    quest.transform.SetParent(quests.transform);
                    quest.GetComponent<RectTransform>().localScale = Vector3.one;
                }

                MelonCoroutines.Start(ResizeQuestGiverGroup(questGiverGroup));

                if (questGiverDetails.Collapsed)
                    MelonCoroutines.Start(DelayedCollapse(quests));
            }

            var flexibleElement = new GameObject("Flexible Element");
            flexibleElement.transform.SetParent(content.transform);
            var flexibleElementRect = flexibleElement.AddComponent<RectTransform>();
            flexibleElementRect.localScale = Vector3.one;
            flexibleElementRect.sizeDelta = Vector2.zero;

            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            if (characterQuests.QuestsPanelCollapsed)
                CollapseToggleQuestsPanel(false);
        }

        public static void QuestProgressChanged(int questId)
        {
            var clientQuest = Global.GetClientQuest(questId);

            if (clientQuest == null || _questsWrapperRect == null)
                return;

            var quest = _questsWrapperRect
                .GetComponentsInChildren<QuestInfoHolder>()
                .FirstOrDefault(quest => quest.QuestId == questId);

            if (quest == null)
                return;

            // Change color of quest on its status
            var headerText = quest.gameObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            headerText.color =
                clientQuest.LooksToBeFailed() ? new Color(1, 0, 0, 1) :
                clientQuest.LooksToBeComplete() ? new Color(0, 1, 0, 1) :
                new Color(1, 1, 1, 1);

            // Loop through all tasks and update
            var tasks = quest.gameObject.transform.GetChild(1);
            for (var i = 0; i < tasks.childCount; i++)
            {
                var uiTask = tasks.GetChild(i);
                var taskProgressText = uiTask.GetChild(0).GetComponent<TextMeshProUGUI>();
                var taskText = uiTask.GetChild(1).GetComponent<TextMeshProUGUI>();
                var clientTask = clientQuest.Tasks.FirstOrDefault(task => task.Text.Equals(taskText.text, StringComparison.OrdinalIgnoreCase));

                if (clientTask == null)
                    continue;

                taskProgressText.text =
                    clientTask.LooksToHaveBeenFailed() ? "Fail" :
                    clientTask.LooksToBeComplete() ? "Done" :
                    $"{clientTask.Progress}/{clientTask.MaxProgress}";

                taskProgressText.color =
                    clientTask.LooksToHaveBeenFailed() ? new Color(1, 0, 0, 1) :
                    clientTask.LooksToBeComplete() ? new Color(0, 1, 0, 1) :
                    new Color(1, 1, 1, 1);
            }
        }

        public static void RemoveQuest(int questId)
        {
            if (_questsWrapperRect == null)
                return;

            var questIdHolder = _questsWrapperRect
                .GetComponentsInChildren<QuestInfoHolder>()
                .FirstOrDefault(id => id.QuestId == questId);

            if (questIdHolder == null)
                return;

            RemoveQuestFromTracking(questId, questIdHolder.gameObject);
        }

        #endregion Public Methods

        #region Private Methods

        private static void CollapseGiver(GameObject group)
        {
            var active = !group.active;
            group.SetActive(active);

            // Enable/Disable layout element to control height
            var questGiverGroup = group.transform.parent.gameObject;
            if (active)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_questsWrapperRect);
                MelonCoroutines.Start(ResizeQuestGiverGroup(questGiverGroup));
            }
            else
            {
                var layoutElement = questGiverGroup.GetComponent<LayoutElement>();
                if (layoutElement)
                    Object.Destroy(layoutElement);

                LayoutRebuilder.ForceRebuildLayoutImmediate(_questsWrapperRect);
            }

            var arrowButtonRect = group.transform.parent.GetChild(0).GetChild(2).GetComponent<RectTransform>();
            arrowButtonRect.pivot = Math.Abs(arrowButtonRect.pivot.y - 1) < 0.0001 ?
                new Vector2(1, 0) :
                new Vector2(1, 1);

            arrowButtonRect.Rotate(new Vector3(180, 0, 0));

            // save that we collapsed to persisted storage
            var giverName = group.transform.parent.gameObject.GetComponents<GiverNameHolder>()[0].GiverName;

            var trackedQuest = Global.TrackedQuests
                .FirstOrDefault(tracked => tracked.GiverName.Equals(giverName, StringComparison.OrdinalIgnoreCase));

            if (trackedQuest != null)
                trackedQuest.Collapsed = !active;

            Global.TrackerModCategory?.SaveToFile(false);
        }

        private static void CollapseToggleQuestsPanel(bool setSetting)
        {
            if (Global.TrackerModCategory == null || Global.QuestTrackerGameObject == null)
                return;

            var playerMod = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            if (setSetting)
            {
                playerMod.QuestsPanelCollapsed = !playerMod.QuestsPanelCollapsed;

                if (playerMod.QuestsPanelCollapsed)
                    playerMod.TrackerSizeDelta = Global.QuestTrackerGameObject
                        .GetComponent<RectTransform>().sizeDelta;

                Global.TrackerModCategory.SaveToFile(false);
            }

            Global.QuestTrackerGameObject.transform
                .Find("Quest Scroll View").gameObject
                .SetActive(!playerMod.QuestsPanelCollapsed);

            Global.QuestTrackerGameObject.GetComponent<RectTransform>().sizeDelta = playerMod.QuestsPanelCollapsed ?
                new Vector2(300, 30) :
                playerMod.TrackerSizeDelta;

            var background = Global.QuestTrackerGameObject.transform.Find("Background");
            background.Find("Resize Bar").gameObject.SetActive(!playerMod.QuestsPanelCollapsed);
            var arrowButtonRect = background.Find("MinMax Button").GetComponent<RectTransform>();
            arrowButtonRect.pivot = Math.Abs(arrowButtonRect.pivot.y - 1) < 0.0001 ?
                new Vector2(0, 0) :
                new Vector2(1, 1);
            arrowButtonRect.Rotate(new Vector3(0, 0, 180));

            // We may be uncollapsing with layouts that weren't able to be built because on start
            // it was collapsed. Here we need to remove them, rebuild, then put them back on.
            if (!playerMod.QuestsPanelCollapsed)
            {
                var questGiverGroups = Global.QuestTrackerGameObject
                    .GetComponentsInChildren<LayoutElement>()
                    .Where(ele => ele.gameObject.name.Equals("Quest Giver Group", StringComparison.OrdinalIgnoreCase))
                    .Select(ele => ele.gameObject)
                    .ToList();

                foreach (var questGiverGroup in questGiverGroups)
                    Object.Destroy(questGiverGroup.GetComponent<LayoutElement>());

                LayoutRebuilder.ForceRebuildLayoutImmediate(_questsWrapperRect);

                foreach (var questGiverGroup in questGiverGroups)
                    MelonCoroutines.Start(ResizeQuestGiverGroup(questGiverGroup));
            }
        }

        private static GameObject CreateQuest(ClientQuest clientQuest)
        {
            var quest = new GameObject("Quest");
            var questRect = quest.AddComponent<RectTransform>();
            questRect.anchoredPosition = new Vector2(20, 0);
            questRect.anchorMax = Vector2.up;
            questRect.anchorMin = Vector2.up;
            questRect.pivot = Vector2.up;
            questRect.localScale = Vector3.one;
            var questVerticalLayoutGroup = quest.AddComponent<VerticalLayoutGroup>();
            questVerticalLayoutGroup.childControlWidth = false;
            questVerticalLayoutGroup.childScaleWidth = false;
            questVerticalLayoutGroup.spacing = 4;
            var questContentSizeFitter = quest.AddComponent<ContentSizeFitter>();
            questContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            questContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var questInfoHolder = quest.AddComponent<QuestInfoHolder>();
            questInfoHolder.QuestId = clientQuest.QuestId;
            questInfoHolder.QuestName = clientQuest.Name;

            var questHeader = new GameObject("Objective Header");
            questHeader.transform.SetParent(quest.transform);
            var questHeaderRect = questHeader.AddComponent<RectTransform>();
            questHeaderRect.anchoredPosition = Vector2.zero;
            questHeaderRect.anchorMax = Vector2.up;
            questHeaderRect.anchorMin = Vector2.up;
            questHeaderRect.pivot = Vector2.up;
            questHeaderRect.sizeDelta = new Vector2(265, 14.51f);
            questHeaderRect.localScale = Vector3.one;
            questHeader.AddComponent<ObjectiveHoverHandler>();
            var questHeaderLayoutElement = questHeader.AddComponent<LayoutElement>();
            questHeaderLayoutElement.preferredHeight = 14.51f;

            var questRemove = new GameObject("Remove Button");
            questRemove.transform.SetParent(questHeader.transform);
            var questRemoveRect = questRemove.AddComponent<RectTransform>();
            questRemoveRect.anchoredPosition = new Vector2(3, -4);
            questRemoveRect.anchorMax = Vector2.up;
            questRemoveRect.anchorMin = Vector2.up;
            questRemoveRect.pivot = Vector2.up;
            questRemoveRect.sizeDelta = new Vector2(8, 8);
            questRemoveRect.localScale = Vector3.one;
            var questRemoveImage = questRemove.AddComponent<Image>();
            var questRemoveTexture = Global.LoadImageToTexture2d("close.png");
            questRemoveTexture.filterMode = FilterMode.Point;
            questRemoveImage.sprite = Sprite.Create(questRemoveTexture,
                new Rect(0, 0, questRemoveTexture.width, questRemoveTexture.height),
                new Vector2(0.5f, 0.5f));
            var questRemoveButton = questRemove.AddComponent<Button>();
            questRemoveButton.onClick.AddListener(new Action(() =>
            {
                RemoveQuestFromTracking(clientQuest.QuestId, quest);
            }));
            var questRemoveLayoutElement = questRemove.AddComponent<LayoutElement>();
            questRemoveLayoutElement.ignoreLayout = true;
            questRemove.SetActive(false);
            var questRemoveTooltip = questRemove.AddComponent<UITooltip>();
            questRemoveTooltip.TooltipText = "Remove quest from tracking";

            var questHeaderText = new GameObject("Header Text");
            questHeaderText.transform.SetParent(questHeader.transform);
            var questHeaderTextRect = questHeaderText.AddComponent<RectTransform>();
            questHeaderTextRect.anchoredPosition = new Vector2(15, 0);
            questHeaderTextRect.anchorMax = Vector2.up;
            questHeaderTextRect.anchorMin = Vector2.up;
            questHeaderTextRect.pivot = Vector2.up;
            questHeaderTextRect.localScale = Vector3.one;
            var questHeaderTextText = questHeaderText.AddComponent<TextMeshProUGUI>();
            var fontResource = Resources
                .FindObjectsOfTypeAll<Font>()
                .FirstOrDefault(resourceFont => resourceFont.name.Equals("Roboto-Bold", StringComparison.OrdinalIgnoreCase));
            var robotoBold = TMP_FontAsset.CreateFontAsset(fontResource);
            questHeaderTextText.text = clientQuest.Name;
            questHeaderTextText.fontSize = 11;
            questHeaderTextText.fontStyle = FontStyles.Normal | FontStyles.Bold;
            questHeaderTextText.color =
                clientQuest.LooksToBeFailed() ? new Color(1, 0, 0, 1) :
                clientQuest.LooksToBeComplete() ? new Color(0, 1, 0, 1) :
                new Color(1, 1, 1, 1);
            questHeaderTextText.font = robotoBold;
            questHeaderTextText.enableWordWrapping = false;
            questHeaderTextText.overflowMode = TextOverflowModes.Ellipsis;
            questHeaderTextRect.sizeDelta = new Vector2(questHeaderTextText.preferredWidth + 0.15f, 14.51f);

            var questWww = new GameObject("WwwLink");
            questWww.transform.SetParent(questHeader.transform);
            var questWwwRect = questWww.AddComponent<RectTransform>();
            questWwwRect.anchoredPosition =
                new Vector2(questHeaderTextRect.anchoredPosition.x + questHeaderTextRect.sizeDelta.x + 4, -2.3f);
            questWwwRect.anchorMax = Vector2.up;
            questWwwRect.anchorMin = Vector2.up;
            questWwwRect.pivot = Vector2.up;
            questWwwRect.sizeDelta = new Vector2(10, 10);
            questWwwRect.localScale = Vector3.one;
            var questWwwImage = questWww.AddComponent<Image>();
            var questWwwTexture = Global.LoadImageToTexture2d("weblink.png");
            questWwwTexture.filterMode = FilterMode.Point;
            questWwwImage.sprite = Sprite.Create(questWwwTexture,
                new Rect(0, 0, questWwwTexture.width, questWwwTexture.height),
                new Vector2(0.5f, 0.5f));
            var questWwwButton = questWww.AddComponent<Button>();
            questWwwButton.onClick.AddListener(new Action(() =>
            {
                Application.OpenURL($"https://shalazam.info/quests?name={WebUtility.UrlEncode(clientQuest.Name.Replace(" ", "+"))}&redirect_if_one=true");
            }));
            var questWwwLayoutElement = questRemove.AddComponent<LayoutElement>();
            questWwwLayoutElement.ignoreLayout = true;
            var questWwwTooltip = questWww.AddComponent<UITooltip>();
            questWwwTooltip.TooltipText = "Open browser to this quest on Shalazam";
            questWww.SetActive(false);

            var questTasks = new GameObject("Tasks");
            questTasks.transform.SetParent(quest.transform);
            var questTasksRect = questTasks.AddComponent<RectTransform>();
            questTasksRect.anchoredPosition = new Vector2(0, 0);
            questTasksRect.anchorMax = Vector2.up;
            questTasksRect.anchorMin = Vector2.up;
            questTasksRect.pivot = Vector2.up;
            questTasksRect.localScale = Vector3.one;
            var questTasksVerticalLayoutGroup = questTasks.AddComponent<VerticalLayoutGroup>();
            questTasksVerticalLayoutGroup.childControlWidth = false;
            questTasksVerticalLayoutGroup.childScaleWidth = false;
            questTasksVerticalLayoutGroup.childForceExpandWidth = false;
            questTasksVerticalLayoutGroup.spacing = 2;
            var questTasksContentSizeFitter = questTasks.AddComponent<ContentSizeFitter>();
            questTasksContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            questTasksContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (var task in clientQuest.Tasks)
            {
                var questText = new GameObject("Quest Text");
                questText.transform.SetParent(questTasks.transform);
                var questTextRect = questText.AddComponent<RectTransform>();
                questTextRect.anchoredPosition = new Vector2(0, 0);
                questTextRect.anchorMax = Vector2.up;
                questTextRect.anchorMin = Vector2.up;
                questTextRect.pivot = Vector2.up;
                questTextRect.localScale = Vector3.one;
                questTextRect.sizeDelta = new Vector2(0, 50);
                var questTextLayoutElement = questText.AddComponent<LayoutElement>();

                var questProgressText = new GameObject("Progress");
                questProgressText.transform.SetParent(questText.transform);
                var questProgressTextRect = questProgressText.AddComponent<RectTransform>();
                questProgressTextRect.anchoredPosition = new Vector2(24, 0);
                questProgressTextRect.anchorMax = Vector2.up;
                questProgressTextRect.anchorMin = Vector2.up;
                questProgressTextRect.pivot = Vector2.up;
                questProgressTextRect.localScale = Vector3.one;
                var progressText = questProgressText.AddComponent<TextMeshProUGUI>();
                var lightFontResource = Resources
                    .FindObjectsOfTypeAll<Font>()
                    .FirstOrDefault(resourceFont => resourceFont.name.Equals("Roboto-Light", StringComparison.OrdinalIgnoreCase));
                var robotoLight = TMP_FontAsset.CreateFontAsset(lightFontResource);
                progressText.text =
                 task.LooksToHaveBeenFailed() ? "Fail" :
                 task.LooksToBeComplete() ? "Done" :
                 $"{task.Progress}/{task.MaxProgress}";
                progressText.fontSize = 11;
                progressText.fontStyle = FontStyles.Bold;
                progressText.color =
                 task.LooksToHaveBeenFailed() ? new Color(1, 0, 0, 1) :
                 task.LooksToBeComplete() ? new Color(0, 1, 0, 1) :
                 new Color(1, 1, 1, 1);
                progressText.font = robotoLight;

                var questTaskText = new GameObject("Text");
                questTaskText.transform.SetParent(questText.transform);
                var questTaskTextRect = questTaskText.AddComponent<RectTransform>();
                questTaskTextRect.anchoredPosition = new Vector2(65, 0);
                questTaskTextRect.anchorMax = Vector2.up;
                questTaskTextRect.anchorMin = Vector2.up;
                questTaskTextRect.pivot = Vector2.up;
                questTaskTextRect.localScale = Vector3.one;
                var questTaskTextText = questTaskText.AddComponent<TextMeshProUGUI>();
                questTaskTextText.text = task.Text;
                questTaskTextText.fontSize = 11;
                questTaskTextText.fontStyle = FontStyles.Bold;
                questTaskTextText.color = new Color(1, 1, 1, 1);
                questTaskTextText.font = robotoLight;
                questTaskTextText.enableWordWrapping = true; // Enable word wrapping
                questTaskTextText.overflowMode = TextOverflowModes.Overflow;

                questTextLayoutElement.preferredHeight = questTaskTextText.preferredHeight;
            }

            return quest;
        }

        private static GameObject CreateQuestGiver(string questGiverName)
        {
            var questGiverGroup = new GameObject("Quest Giver Group");
            var questGiverGroupRect = questGiverGroup.AddComponent<RectTransform>();
            questGiverGroupRect.anchoredPosition = Vector2.zero;
            questGiverGroupRect.anchorMax = Vector2.up;
            questGiverGroupRect.anchorMin = Vector2.up;
            questGiverGroupRect.pivot = Vector2.up;
            var questGiverGroupVerticalLayoutGroup = questGiverGroup.AddComponent<VerticalLayoutGroup>();
            questGiverGroupVerticalLayoutGroup.childForceExpandHeight = false;
            questGiverGroupVerticalLayoutGroup.childControlWidth = false;
            questGiverGroupVerticalLayoutGroup.childScaleWidth = false;
            questGiverGroupVerticalLayoutGroup.childForceExpandWidth = false;
            questGiverGroupVerticalLayoutGroup.spacing = 22;
            var questGiverGroupContentSizeFitter = questGiverGroup.AddComponent<ContentSizeFitter>();
            questGiverGroupContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            questGiverGroupContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var questGiverGroupGiverNameHolder = questGiverGroup.AddComponent<GiverNameHolder>();
            questGiverGroupGiverNameHolder.GiverName = questGiverName;

            // Quest Giver
            var questGiver = new GameObject("Quest Giver Header");
            questGiver.transform.SetParent(questGiverGroup.transform);
            var questGiverRect = questGiver.AddComponent<RectTransform>();
            questGiverRect.anchoredPosition = Vector2.zero;
            questGiverRect.anchorMax = Vector2.up;
            questGiverRect.anchorMin = Vector2.up;
            questGiverRect.pivot = Vector2.up;
            questGiverRect.localScale = Vector3.one;
            questGiverRect.sizeDelta = new Vector2(265, 0);

            var questGiverBackground = new GameObject("Background");
            questGiverBackground.transform.SetParent(questGiver.transform);
            var questGiverBackgroundRect = questGiverBackground.AddComponent<RectTransform>();
            questGiverBackgroundRect.anchoredPosition = Vector2.zero;
            questGiverBackgroundRect.anchorMax = Vector2.one;
            questGiverBackgroundRect.anchorMin = Vector2.up;
            questGiverBackgroundRect.pivot = Vector2.up;
            questGiverBackgroundRect.localScale = Vector3.one;
            questGiverBackgroundRect.sizeDelta = new Vector2(0, 20);
            var questGiverBackgroundImage = questGiverBackground.AddComponent<Image>();
            var questGiverBackgroundImageTexture = Global.LoadImageToTexture2d("BlueBackground.png");
            questGiverBackgroundImageTexture.filterMode = FilterMode.Point;
            questGiverBackgroundImage.sprite = Sprite.Create(questGiverBackgroundImageTexture,
             new Rect(0, 0, questGiverBackgroundImageTexture.width, questGiverBackgroundImageTexture.height),
             new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight,
             new Vector4(12, 12, 12, 12));
            questGiverBackgroundImage.type = Image.Type.Sliced;
            var questGiverBackgroundLayoutElement = questGiverBackground.AddComponent<LayoutElement>();
            questGiverBackgroundLayoutElement.ignoreLayout = true;

            var questGiverText = new GameObject("Text");
            questGiverText.transform.SetParent(questGiver.transform);
            var questGiverTextRect = questGiverText.AddComponent<RectTransform>();
            questGiverTextRect.anchoredPosition = new Vector2(6, -3);
            questGiverTextRect.anchorMax = Vector2.up;
            questGiverTextRect.anchorMin = Vector2.up;
            questGiverTextRect.pivot = Vector2.up;
            questGiverTextRect.localScale = Vector3.one;
            questGiverTextRect.sizeDelta = new Vector2(200, 20);
            var giverFontResource = Resources
                .FindObjectsOfTypeAll<Font>()
                .FirstOrDefault(resourceFont => resourceFont.name.Equals("OptimusPrincepsSemiBold", StringComparison.OrdinalIgnoreCase));
            var giverFont = TMP_FontAsset.CreateFontAsset(giverFontResource);
            var questGiverHeaderText = questGiverText.AddComponent<TextMeshProUGUI>();
            questGiverHeaderText.text = questGiverName;
            questGiverHeaderText.fontSize = 13;
            questGiverHeaderText.fontStyle = FontStyles.Bold;
            questGiverHeaderText.color = new Color(1, 1, 1, 1);
            questGiverHeaderText.font = giverFont;
            questGiverHeaderText.enableWordWrapping = false;
            questGiverHeaderText.overflowMode = TextOverflowModes.Ellipsis;

            var questGiverCollapse = new GameObject("Collapse");
            questGiverCollapse.transform.SetParent(questGiver.transform);
            var questGiverCollapseRect = questGiverCollapse.AddComponent<RectTransform>();
            questGiverCollapseRect.anchoredPosition = new Vector2(-6, -4);
            questGiverCollapseRect.anchorMax = Vector2.one;
            questGiverCollapseRect.anchorMin = Vector2.one;
            questGiverCollapseRect.pivot = Vector2.one;
            questGiverCollapseRect.localScale = Vector3.one;
            questGiverCollapseRect.sizeDelta = new Vector2(12, 12);
            var questGiverCollapseRenderer = questGiverCollapse.AddComponent<CanvasRenderer>();
            questGiverCollapseRenderer.cullTransparentMesh = false;
            var questGiverCollapseImage = questGiverCollapse.AddComponent<Image>();
            var questGiverCollapseTexture = Global.LoadImageToTexture2d("chev_button.png");
            questGiverCollapseTexture.filterMode = FilterMode.Point;
            questGiverCollapseImage.sprite = Sprite.Create(questGiverCollapseTexture,
             new Rect(0, 0, questGiverCollapseTexture.width, questGiverCollapseTexture.height),
             new Vector2(0.5f, 0.5f));
            var questGiverCollapseButton = questGiver.AddComponent<Button>();
            var questGiverCollapseLayoutElement = questGiverCollapse.AddComponent<LayoutElement>();
            questGiverCollapseLayoutElement.ignoreLayout = true;

            var quests = new GameObject("Quests");
            quests.transform.SetParent(questGiverGroup.transform);
            var questsRect = quests.AddComponent<RectTransform>();
            questsRect.anchoredPosition = new Vector2(0, 0);
            questsRect.anchorMax = Vector2.up;
            questsRect.anchorMin = Vector2.up;
            questsRect.pivot = Vector2.up;
            questsRect.localScale = Vector3.one;
            var questsVerticalLayoutGroup = quests.AddComponent<VerticalLayoutGroup>();
            questsVerticalLayoutGroup.childControlWidth = false;
            questsVerticalLayoutGroup.childScaleWidth = false;
            questsVerticalLayoutGroup.childForceExpandWidth = false;
            questsVerticalLayoutGroup.spacing = 4;
            var questsContentSizeFitter = quests.AddComponent<ContentSizeFitter>();
            questsContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            questsContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            questGiverCollapseButton.onClick.AddListener(new Action(() => CollapseGiver(quests)));

            return questGiverGroup;
        }

        private static IEnumerator DelayedCollapse(GameObject group)
        {
            yield return null;

            CollapseGiver(group);
        }

        private static void RebuildAll()
        {
            if (Global.QuestTrackerGameObject == null)
                return;

            foreach (var rect in Global.QuestTrackerGameObject
                         .GetComponentsInChildren<RectTransform>()
                         .Reverse())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        private static IEnumerator RebuildLayoutDelayed()
        {
            yield return null; // Wait for one frame
            if (Global.QuestTrackerGameObject == null)
                yield break;

            RebuildAll();
        }

        private static void RemoveQuestFromTracking(int questId, GameObject quest)
        {
            var quests = quest.gameObject.transform.parent.gameObject;

            if (quests.transform.GetChildCount() > 1)
            {
                quest.SetActive(false);
                RebuildAll();
                Object.Destroy(quest);
            }
            else
            {
                Object.Destroy(quests.transform.parent.gameObject);
            }

            ModQuest? questGiver = null;
            foreach (var trackedQuest in Global.TrackedQuests)
            {
                var questIdFound = trackedQuest.TrackedQuestIds
                    .Any(id => id == questId);

                if (!questIdFound)
                    continue;

                questGiver = trackedQuest;
                break;
            }

            if (questGiver != null)
            {
                // Remove the quest from the giver
                questGiver.TrackedQuestIds = questGiver.TrackedQuestIds
                    .Where(id => id != questId)
                    .ToList();

                if (questGiver.TrackedQuestIds.Count < 1)
                    Global.TrackedQuests.Remove(questGiver);
            }

            Global.TrackerModCategory?.SaveToFile(false);

            UiQuestJournal.RecheckCheckmarks();

            if (_questsWrapperRect == null)
                return;

            // For sizing the layouts
            var questGiverGroup = quests.transform.parent.gameObject;
            Object.Destroy(questGiverGroup.GetComponent<LayoutElement>());
            MelonCoroutines.Start(RebuildLayoutDelayed());
            MelonCoroutines.Start(ResizeQuestGiverGroup(questGiverGroup));
        }

        private static IEnumerator ResizeQuestGiverGroup(GameObject? questGiverGroup)
        {
            yield return null;

            if (questGiverGroup == null)
                yield break;

            var questGiverGroupLayoutElement = questGiverGroup.AddComponent<LayoutElement>();
            questGiverGroupLayoutElement.preferredHeight = questGiverGroup.GetComponent<RectTransform>().sizeDelta.y - 14;
        }

        #endregion Private Methods
    }
}