using Il2Cpp;
using Il2CppTMPro;
using PantheonQuestProgressTrackerMod.Components;
using PantheonQuestProgressTrackerMod.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PantheonQuestProgressTrackerMod.Panels
{
    internal static class TrackerPanelSettings
    {
        #region Private Fields

        private static TMP_FontAsset? _robotoBold;
        private static UIWindowPanel? _settingsUiWindowPanel;

        #endregion Private Fields

        #region Public Methods

        public static void CreateSettingsPanel(Transform midPanel)
        {
            if (_settingsUiWindowPanel != null)
                return;

            var fontResource = Resources
                .FindObjectsOfTypeAll<Font>()
                .FirstOrDefault(resourceFont => resourceFont.name.Equals("Roboto-Bold", StringComparison.OrdinalIgnoreCase));
            _robotoBold = TMP_FontAsset.CreateFontAsset(fontResource);

            var trackerSettings = new GameObject("Panel_QuestProgressTrackerSettings");
            trackerSettings.transform.SetParent(midPanel);
            trackerSettings.layer = Layers.UI;
            var trackerSettingsRect = trackerSettings.AddComponent<RectTransform>();
            trackerSettingsRect.sizeDelta = new Vector2(240, 250);
            trackerSettingsRect.localScale = Vector3.one;
            trackerSettingsRect.anchorMin = new Vector2(0.5f, 0.5f);
            trackerSettingsRect.anchorMax = new Vector2(0.5f, 0.5f);
            trackerSettingsRect.anchoredPosition = Vector2.zero;
            var trackerSettingsPreventMouseImage = trackerSettings.AddComponent<Image>();
            trackerSettingsPreventMouseImage.color = new Color(0, 0, 0, 0);
            trackerSettingsPreventMouseImage.raycastTarget = true;
            trackerSettings.AddComponent<GraphicRaycaster>();
            trackerSettings.AddComponent<UIDraggable>();
            _settingsUiWindowPanel = trackerSettings.AddComponent<UIWindowPanel>();
            var canvasGroup = trackerSettings.AddComponent<CanvasGroup>();
            _settingsUiWindowPanel.CanvasGroup = canvasGroup;
            _settingsUiWindowPanel._displayName = "";

            // Panel background
            var trackerSettingsBackground = new GameObject("Background");
            trackerSettingsBackground.transform.SetParent(trackerSettings.transform);
            var trackerSettingsBackgroundRect = trackerSettingsBackground.AddComponent<RectTransform>();
            trackerSettingsBackgroundRect.anchoredPosition = new Vector2(1.5f, -1.5f);
            trackerSettingsBackgroundRect.anchorMax = new Vector2(1, 1);
            trackerSettingsBackgroundRect.anchorMin = new Vector2(0, 0);
            trackerSettingsBackgroundRect.sizeDelta = new Vector2(-1, -1);
            trackerSettingsBackgroundRect.localScale = Vector3.one;
            var trackerSettingsBackgroundCanvasRenderer = trackerSettingsBackground.AddComponent<CanvasRenderer>();
            trackerSettingsBackgroundCanvasRenderer.cullTransparentMesh = false;
            var trackerSettingsBackgroundImage = trackerSettingsBackground.AddComponent<Image>();
            var trackerSettingsBackgroundImageTexture = Global.LoadImageToTexture2d("InventoryBag_Bkg.png");
            trackerSettingsBackgroundImage.sprite = Sprite.Create(trackerSettingsBackgroundImageTexture,
                new Rect(0, 0, trackerSettingsBackgroundImageTexture.width, trackerSettingsBackgroundImageTexture.height),
                new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.Tight,
                new Vector4(6, 7, 16, 14));
            trackerSettingsBackgroundImage.type = Image.Type.Sliced;
            var backgroundLayoutElement = trackerSettingsBackground.AddComponent<LayoutElement>();
            backgroundLayoutElement.ignoreLayout = true;

            // Close button of the background
            var backgroundCancelButton = new GameObject("CancelButton");
            backgroundCancelButton.transform.SetParent(trackerSettingsBackground.transform, false);
            var backgroundCancelRect = backgroundCancelButton.AddComponent<RectTransform>();
            backgroundCancelRect.anchoredPosition = new Vector2(-14f, -13f);
            backgroundCancelRect.anchorMax = new Vector2(1f, 1f);
            backgroundCancelRect.anchorMin = new Vector2(1f, 1f);
            backgroundCancelRect.sizeDelta = new Vector2(30f, 30f);
            backgroundCancelRect.localScale = Vector3.one;
            var backgroundCancelRenderer = backgroundCancelButton.AddComponent<CanvasRenderer>();
            backgroundCancelRenderer.cullTransparentMesh = false;
            var backgroundCancelButtonImage = backgroundCancelButton.AddComponent<Image>();
            var backgroundCancelButtonTexture = Global.LoadImageToTexture2d("CodexCloseBtn.png");
            backgroundCancelButtonImage.sprite = Sprite.Create(backgroundCancelButtonTexture,
                new Rect(0, 0, backgroundCancelButtonTexture.width, backgroundCancelButtonTexture.height),
                new Vector2(0.5f, 0.5f));
            var cancelButton = backgroundCancelButton.AddComponent<Button>();
            cancelButton.onClick.AddListener(new Action(() =>
            {
                Global.TrackerModCategory?.SaveToFile(false);
                _settingsUiWindowPanel.Hide();
            }));
            var backgroundCancelLayoutElement = backgroundCancelButton.AddComponent<LayoutElement>();
            backgroundCancelLayoutElement.ignoreLayout = true;

            // Panel title
            var settingsTitle = new GameObject("Title");
            settingsTitle.transform.SetParent(trackerSettingsBackground.transform);
            var settingsTitleRect = settingsTitle.AddComponent<RectTransform>();
            settingsTitleRect.anchoredPosition = new Vector2(0, -12);
            settingsTitleRect.anchorMax = new Vector2(0.5f, 1);
            settingsTitleRect.anchorMin = new Vector2(0.5f, 1);
            settingsTitleRect.pivot = new Vector2(0.5f, 1);
            settingsTitleRect.sizeDelta = Vector2.zero;
            settingsTitleRect.localScale = Vector3.one;
            var settingsTitleFirstRow = new GameObject("First");
            settingsTitleFirstRow.transform.SetParent(settingsTitle.transform);
            var settingsTitleFirstRowRect = settingsTitleFirstRow.AddComponent<RectTransform>();
            settingsTitleFirstRowRect.anchoredPosition = Vector2.zero;
            settingsTitleFirstRowRect.anchorMax = Vector2.one;
            settingsTitleFirstRowRect.anchorMin = Vector2.zero;
            settingsTitleFirstRowRect.pivot = new Vector2(0.5f, 1);
            settingsTitleFirstRowRect.localScale = Vector3.one;
            var questProgressTrackerText = settingsTitleFirstRow.AddComponent<TextMeshProUGUI>();
            questProgressTrackerText.text = "Quest Progress Tracker";
            questProgressTrackerText.fontSize = 14;
            questProgressTrackerText.fontStyle = FontStyles.Bold;
            questProgressTrackerText.color = new Color(250f / 255, 212f / 255, 13f / 255, 1);
            questProgressTrackerText.font = _robotoBold!;
            settingsTitleFirstRowRect.sizeDelta = new Vector2(questProgressTrackerText.preferredWidth, 0);
            var settingsTitleSecondRow = new GameObject("Second");
            settingsTitleSecondRow.transform.SetParent(settingsTitle.transform);
            var settingsTitleSecondRowRect = settingsTitleSecondRow.AddComponent<RectTransform>();
            settingsTitleSecondRowRect.anchoredPosition = new Vector2(0, -20);
            settingsTitleSecondRowRect.anchorMax = Vector2.one;
            settingsTitleSecondRowRect.anchorMin = Vector2.zero;
            settingsTitleSecondRowRect.pivot = new Vector2(0.5f, 1);
            settingsTitleSecondRowRect.localScale = Vector3.one;
            var settingsTitleText = settingsTitleSecondRow.AddComponent<TextMeshProUGUI>();
            settingsTitleText.text = "Settings";
            settingsTitleText.fontSize = 14;
            settingsTitleText.fontStyle = FontStyles.Bold;
            settingsTitleText.color = new Color(250f / 255, 212f / 255, 13f / 255, 1);
            settingsTitleText.font = _robotoBold!;
            settingsTitleSecondRowRect.sizeDelta = new Vector2(settingsTitleText.preferredWidth, 0);

            var settingsContainer = new GameObject("Settings Container");
            settingsContainer.transform.SetParent(trackerSettings.transform);
            var settingsContainerRect = settingsContainer.AddComponent<RectTransform>();
            settingsContainerRect.anchoredPosition = new Vector2(0, -65);
            settingsContainerRect.anchorMax = Vector2.one;
            settingsContainerRect.anchorMin = new Vector2(0, 0.5f);
            settingsContainerRect.pivot = new Vector2(0.5f, 0.5f);
            settingsContainerRect.sizeDelta = Vector2.zero;
            settingsContainerRect.localScale = Vector3.one;

            SetupScaleSetting(settingsContainer);
            SetupLockPositionSetting(settingsContainer);
            SetupAlwaysTrackNewQuestsSetting(settingsContainer);
            SetupSolidBackground(settingsContainer);
            SetupSolidBackgroundOpacity(settingsContainer);
        }

        public static void OpenPanelToggle()
        {
            if (_settingsUiWindowPanel == null)
                return;

            if (_settingsUiWindowPanel.IsVisible)
            {
                _settingsUiWindowPanel.Hide();
                Global.TrackerModCategory?.SaveToFile(false);
            }
            else
                _settingsUiWindowPanel.Show();
        }

        #endregion Public Methods

        #region Private Methods

        private static void ScaleSettingChanged(Slider slider, TextMeshProUGUI text)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var sliderValue = slider.value;
            text.text = $"Scale: {sliderValue}";
            characterQuests.TrackerScale = sliderValue;

            if (Global.QuestTrackerGameObject == null)
                return;

            Global.QuestTrackerGameObject.GetComponent<RectTransform>().localScale = new Vector3(sliderValue / 100, sliderValue / 100, sliderValue / 100);
        }

        private static void SetupAlwaysTrackNewQuestsSetting(GameObject attachTo)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var trackNewQuests = characterQuests.AlwaysTrackNewQuests;

            var trackNew = new GameObject("Always Track New Setting");
            trackNew.transform.SetParent(attachTo.transform);
            var trackNewGroupRect = trackNew.AddComponent<RectTransform>();
            trackNewGroupRect.anchoredPosition = new Vector2(0, -95);
            trackNewGroupRect.anchorMax = Vector2.up;
            trackNewGroupRect.anchorMin = Vector2.up;
            trackNewGroupRect.pivot = Vector2.up;
            trackNewGroupRect.localScale = Vector3.one;
            var trackNewToggle = trackNew.AddComponent<Toggle>();
            trackNewToggle.isOn = trackNewQuests;
            trackNewToggle.onValueChanged.AddListener(new Action<bool>(_ =>
            {
                characterQuests.AlwaysTrackNewQuests = trackNewToggle.isOn;
            }));

            var trackNewTitle = new GameObject("Title");
            trackNewTitle.transform.SetParent(trackNew.transform);
            var trackNewTitleRect = trackNewTitle.AddComponent<RectTransform>();
            trackNewTitleRect.anchoredPosition = new Vector2(70, 45);
            trackNewTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            trackNewTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            trackNewTitleRect.pivot = new Vector2(0.5f, 0.5f);
            trackNewTitleRect.localScale = Vector3.one;
            var trackNewTitleText = trackNewTitle.AddComponent<TextMeshProUGUI>();
            trackNewTitleText.text = "Auto Track New Quests:";
            trackNewTitleText.fontSize = 12;
            trackNewTitleText.fontStyle = FontStyles.Bold;
            trackNewTitleText.color = new Color(1, 1, 1, 1);
            trackNewTitleText.font = _robotoBold!;

            var trackNewCheck = new GameObject("Checkbox");
            trackNewCheck.transform.SetParent(trackNew.transform);
            var trackNewCheckRect = trackNewCheck.AddComponent<RectTransform>();
            trackNewCheckRect.anchoredPosition = new Vector2(85, 3);
            trackNewCheckRect.anchorMax = Vector2.one;
            trackNewCheckRect.anchorMin = Vector2.one;
            trackNewCheckRect.pivot = Vector2.zero;
            trackNewCheckRect.sizeDelta = new Vector2(16, 16);
            trackNewCheckRect.localScale = Vector3.one;
            var trackNewCheckBackground = trackNewCheck.AddComponent<Image>();
            var trackNewCheckBackgroundTexture = Global.LoadImageToTexture2d("CheckBox_Frame.png");
            trackNewCheckBackgroundTexture.filterMode = FilterMode.Point;
            trackNewCheckBackground.sprite = Sprite.Create(trackNewCheckBackgroundTexture,
                new Rect(0, 0, trackNewCheckBackgroundTexture.width, trackNewCheckBackgroundTexture.height),
                new Vector2(0.5f, 0.5f));

            var trackNewCheckmark = new GameObject("Checkmark");
            trackNewCheckmark.transform.SetParent(trackNewCheck.transform);
            var trackNewCheckmarkRect = trackNewCheckmark.AddComponent<RectTransform>();
            trackNewCheckmarkRect.anchoredPosition = Vector2.zero;
            trackNewCheckmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            trackNewCheckmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            trackNewCheckmarkRect.pivot = new Vector2(0.5f, 0.5f);
            trackNewCheckmarkRect.sizeDelta = new Vector2(16, 16);
            trackNewCheckmarkRect.localScale = Vector3.one;
            var trackNewCheckmarkImage = trackNewCheckmark.AddComponent<Image>();
            var trackNewCheckmarkImageTexture = Global.LoadImageToTexture2d("CheckBox_Tick.png");
            trackNewCheckmarkImageTexture.filterMode = FilterMode.Point;
            trackNewCheckmarkImage.sprite = Sprite.Create(trackNewCheckmarkImageTexture,
                new Rect(0, 0, trackNewCheckmarkImageTexture.width, trackNewCheckmarkImageTexture.height),
                new Vector2(0.5f, 0.5f));

            trackNewToggle.graphic = trackNewCheckmarkImage;
            trackNewToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        }

        private static void SetupLockPositionSetting(GameObject attachTo)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var lockPositionGroup = new GameObject("Lock Position Setting");
            lockPositionGroup.transform.SetParent(attachTo.transform);
            var lockPositionGroupRect = lockPositionGroup.AddComponent<RectTransform>();
            lockPositionGroupRect.anchoredPosition = new Vector2(0, -65);
            lockPositionGroupRect.anchorMax = Vector2.up;
            lockPositionGroupRect.anchorMin = Vector2.up;
            lockPositionGroupRect.pivot = Vector2.up;
            lockPositionGroupRect.localScale = Vector3.one;
            var lockPositionToggle = lockPositionGroup.AddComponent<Toggle>();
            lockPositionToggle.isOn = characterQuests.LockPosition;
            lockPositionToggle.onValueChanged.AddListener(new Action<bool>(_ =>
            {
                characterQuests.LockPosition = lockPositionToggle.isOn;

                if (Global.QuestTrackerGameObject == null)
                    return;

                Global.QuestTrackerGameObject.GetComponent<UIDraggable>().enabled = !lockPositionToggle.isOn;
            }));

            var lockPositionTitle = new GameObject("Title");
            lockPositionTitle.transform.SetParent(lockPositionGroup.transform);
            var lockPositionTitleRect = lockPositionTitle.AddComponent<RectTransform>();
            lockPositionTitleRect.anchoredPosition = new Vector2(70, 45);
            lockPositionTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            lockPositionTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            lockPositionTitleRect.pivot = new Vector2(0.5f, 0.5f);
            lockPositionTitleRect.localScale = Vector3.one;
            var lockPositionTitleText = lockPositionTitle.AddComponent<TextMeshProUGUI>();
            lockPositionTitleText.text = "Lock Quests Position:";
            lockPositionTitleText.fontSize = 12;
            lockPositionTitleText.fontStyle = FontStyles.Bold;
            lockPositionTitleText.color = new Color(1, 1, 1, 1);
            lockPositionTitleText.font = _robotoBold!;

            var lockPositionCheck = new GameObject("Checkbox");
            lockPositionCheck.transform.SetParent(lockPositionGroup.transform);
            var lockPositionCheckRect = lockPositionCheck.AddComponent<RectTransform>();
            lockPositionCheckRect.anchoredPosition = new Vector2(85, 3);
            lockPositionCheckRect.anchorMax = Vector2.one;
            lockPositionCheckRect.anchorMin = Vector2.one;
            lockPositionCheckRect.pivot = Vector2.zero;
            lockPositionCheckRect.sizeDelta = new Vector2(16, 16);
            lockPositionCheckRect.localScale = Vector3.one;
            var lockPositionCheckBackground = lockPositionCheck.AddComponent<Image>();
            var lockPositionCheckBackgroundTexture = Global.LoadImageToTexture2d("CheckBox_Frame.png");
            lockPositionCheckBackgroundTexture.filterMode = FilterMode.Point;
            lockPositionCheckBackground.sprite = Sprite.Create(lockPositionCheckBackgroundTexture,
                new Rect(0, 0, lockPositionCheckBackgroundTexture.width, lockPositionCheckBackgroundTexture.height),
                new Vector2(0.5f, 0.5f));

            var lockPositionCheckmark = new GameObject("Checkmark");
            lockPositionCheckmark.transform.SetParent(lockPositionCheck.transform);
            var lockPositionCheckmarkRect = lockPositionCheckmark.AddComponent<RectTransform>();
            lockPositionCheckmarkRect.anchoredPosition = Vector2.zero;
            lockPositionCheckmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            lockPositionCheckmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            lockPositionCheckmarkRect.pivot = new Vector2(0.5f, 0.5f);
            lockPositionCheckmarkRect.sizeDelta = new Vector2(16, 16);
            lockPositionCheckmarkRect.localScale = Vector3.one;
            var lockPositionCheckmarkImage = lockPositionCheckmark.AddComponent<Image>();
            var lockPositionCheckmarkImageTexture = Global.LoadImageToTexture2d("CheckBox_Tick.png");
            lockPositionCheckmarkImageTexture.filterMode = FilterMode.Point;
            lockPositionCheckmarkImage.sprite = Sprite.Create(lockPositionCheckmarkImageTexture,
                new Rect(0, 0, lockPositionCheckmarkImageTexture.width, lockPositionCheckmarkImageTexture.height),
                new Vector2(0.5f, 0.5f));

            lockPositionToggle.graphic = lockPositionCheckmarkImage;
            lockPositionToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        }

        private static void SetupScaleSetting(GameObject attachTo)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var scaleSettingGroup = new GameObject("Scale Setting");
            scaleSettingGroup.transform.SetParent(attachTo.transform);
            var scaleSettingGroupRect = scaleSettingGroup.AddComponent<RectTransform>();
            scaleSettingGroupRect.anchoredPosition = new Vector2(0, -20);
            scaleSettingGroupRect.anchorMax = Vector2.up;
            scaleSettingGroupRect.anchorMin = Vector2.up;
            scaleSettingGroupRect.pivot = Vector2.up;
            scaleSettingGroupRect.localScale = Vector3.one;

            var scaleSettingTitle = new GameObject("Title");
            scaleSettingTitle.transform.SetParent(scaleSettingGroup.transform);
            var scaleSettingTitleRect = scaleSettingTitle.AddComponent<RectTransform>();
            scaleSettingTitleRect.anchoredPosition = new Vector2(70, 45);
            scaleSettingTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            scaleSettingTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            scaleSettingTitleRect.pivot = new Vector2(0.5f, 0.5f);
            scaleSettingTitleRect.localScale = Vector3.one;
            var scaleSettingTitleText = scaleSettingTitle.AddComponent<TextMeshProUGUI>();
            scaleSettingTitleText.text = $"Scale: {characterQuests.TrackerScale}";
            scaleSettingTitleText.fontSize = 12;
            scaleSettingTitleText.fontStyle = FontStyles.Bold;
            scaleSettingTitleText.color = new Color(1, 1, 1, 1);
            scaleSettingTitleText.font = _robotoBold!;

            var scaleSetting = new GameObject("Scale Slider");
            scaleSetting.transform.SetParent(scaleSettingGroup.transform);
            var scaleSettingSliderRect = scaleSetting.AddComponent<RectTransform>();
            scaleSettingSliderRect.anchoredPosition = new Vector2(70, 45);
            scaleSettingSliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            scaleSettingSliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            scaleSettingSliderRect.pivot = new Vector2(0.5f, 0.5f);
            scaleSettingSliderRect.localScale = Vector3.one;
            scaleSettingSliderRect.sizeDelta = new Vector2(200, 20);
            var scaleSlider = scaleSetting.AddComponent<Slider>();
            scaleSlider.maxValue = 150;
            scaleSlider.minValue = 50;
            scaleSlider.normalizedValue = 0.7f;
            scaleSlider.value = characterQuests.TrackerScale;
            scaleSlider.wholeNumbers = true;
            scaleSlider.onValueChanged.AddListener(new Action<float>(_ => ScaleSettingChanged(scaleSlider, scaleSettingTitleText)));

            var scaleSliderBackground = new GameObject("Background");
            scaleSliderBackground.transform.SetParent(scaleSetting.transform);
            var scaleSliderBackgroundRect = scaleSliderBackground.AddComponent<RectTransform>();
            scaleSliderBackgroundRect.anchoredPosition = Vector2.zero;
            scaleSliderBackgroundRect.anchorMax = new Vector2(1, 0.75f);
            scaleSliderBackgroundRect.anchorMin = new Vector2(0, 0.25f);
            scaleSliderBackgroundRect.pivot = new Vector2(0.5f, 0.5f);
            scaleSliderBackgroundRect.localScale = Vector3.one;
            scaleSliderBackgroundRect.sizeDelta = Vector2.zero;
            var scaleSliderBackgroundImage = scaleSliderBackground.AddComponent<Image>();
            scaleSliderBackgroundImage.color = new Color(1, 1, 1, 1);

            var scaleSliderFillArea = new GameObject("Fill Area");
            scaleSliderFillArea.transform.SetParent(scaleSetting.transform);
            var scaleSliderFillAreaRect = scaleSliderFillArea.AddComponent<RectTransform>();
            scaleSliderFillAreaRect.anchoredPosition = new Vector2(-5, 0);
            scaleSliderFillAreaRect.anchorMax = new Vector2(1, 0.75f);
            scaleSliderFillAreaRect.anchorMin = new Vector2(0, 0.25f);
            scaleSliderFillAreaRect.pivot = new Vector2(0.5f, 0.5f);
            scaleSliderFillAreaRect.localScale = Vector3.one;
            scaleSliderFillAreaRect.sizeDelta = new Vector2(-20, 0);
            var scaleSliderFillAreaFill = new GameObject("Fill");
            scaleSliderFillAreaFill.transform.SetParent(scaleSliderFillArea.transform);
            var scaleSliderFillAreaFillRect = scaleSliderFillAreaFill.AddComponent<RectTransform>();
            scaleSliderFillAreaFillRect.anchoredPosition = Vector2.zero;
            scaleSliderFillAreaFillRect.anchorMax = new Vector2(0.51f, 1);
            scaleSliderFillAreaFillRect.anchorMin = Vector2.zero;
            scaleSliderFillAreaFillRect.pivot = new Vector2(0.5f, 0.5f);
            scaleSliderFillAreaFillRect.localScale = Vector3.one;
            scaleSliderFillAreaFillRect.sizeDelta = new Vector2(10, 0);
            var scaleSliderFillAreaFillImage = scaleSliderFillAreaFill.AddComponent<Image>();
            scaleSliderFillAreaFillImage.color = new Color(1, 1, 1, 1);
            scaleSlider.fillRect = scaleSliderFillAreaFillRect;

            var scaleSliderHandle = new GameObject("Handle");
            scaleSliderHandle.transform.SetParent(scaleSetting.transform);
            var scaleSliderHandleRect = scaleSliderHandle.AddComponent<RectTransform>();
            scaleSliderHandleRect.anchoredPosition = Vector2.zero;
            scaleSliderHandleRect.anchorMax = Vector2.one;
            scaleSliderHandleRect.anchorMin = Vector2.zero;
            scaleSliderHandleRect.pivot = new Vector2(0.5f, 0.5f);
            scaleSliderHandleRect.localScale = Vector3.one;
            scaleSliderHandleRect.sizeDelta = new Vector2(-20, 0);
            var scaleSliderHandleHandle = new GameObject("Handle");
            scaleSliderHandleHandle.transform.SetParent(scaleSliderHandle.transform);
            var scaleSliderHandleHandleRect = scaleSliderHandleHandle.AddComponent<RectTransform>();
            scaleSliderHandleHandleRect.anchoredPosition = Vector2.zero;
            scaleSliderHandleHandleRect.anchorMax = new Vector2(0.51f, 1);
            scaleSliderHandleHandleRect.anchorMin = new Vector2(0.51f, 0);
            scaleSliderHandleHandleRect.pivot = new Vector2(0.5f, 0.5f);
            scaleSliderHandleHandleRect.sizeDelta = new Vector2(10, 0);
            scaleSliderHandleHandleRect.localScale = Vector3.one;
            var scaleSliderHandleHandleImage = scaleSliderHandleHandle.AddComponent<Image>();
            var scaleSliderHandleHandleTexture = Global.LoadImageToTexture2d("ScrollBarVertical.png");
            scaleSliderHandleHandleImage.sprite = Sprite.Create(scaleSliderHandleHandleTexture,
                new Rect(0, 0, scaleSliderHandleHandleTexture.width, scaleSliderHandleHandleTexture.height),
                new Vector2(0.5f, 0.5f));
            scaleSlider.handleRect = scaleSliderHandleHandleRect;
            scaleSlider.image = scaleSliderHandleHandleImage;
            scaleSlider.targetGraphic = scaleSliderHandleHandleImage;
        }

        private static void SetupSolidBackground(GameObject attachTo)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var solidBackgroundValue = characterQuests.SolidBackground;

            var solidBackground = new GameObject("Solid Background");
            solidBackground.transform.SetParent(attachTo.transform);
            var solidBackgroundGroupRect = solidBackground.AddComponent<RectTransform>();
            solidBackgroundGroupRect.anchoredPosition = new Vector2(0, -125);
            solidBackgroundGroupRect.anchorMax = Vector2.up;
            solidBackgroundGroupRect.anchorMin = Vector2.up;
            solidBackgroundGroupRect.pivot = Vector2.up;
            solidBackgroundGroupRect.localScale = Vector3.one;
            var solidBackgroundToggle = solidBackground.AddComponent<Toggle>();
            solidBackgroundToggle.isOn = solidBackgroundValue;
            solidBackgroundToggle.onValueChanged.AddListener(new Action<bool>(_ =>
            {
                characterQuests.SolidBackground = solidBackgroundToggle.isOn;
                Global.SolidBackground = solidBackgroundToggle.isOn;

                if (Global.QuestTrackerGameObject == null)
                    return;

                var opacityHandlers = Global.QuestTrackerGameObject
                    .GetComponentsInChildren<ImageOpacityHandler>();

                foreach (var handler in opacityHandlers)
                {
                    var image = handler.gameObject.GetComponent<Image>();

                    if (image == null)
                        continue;

                    if (handler.MainBackground)
                    {
                        image.color = solidBackgroundToggle.isOn ?
                            image.color with { a = characterQuests.SolidBackgroundOpacity / 100 } :
                            image.color with { a = 0.27f };
                    }
                    else
                    {
                        image.color = solidBackgroundToggle.isOn ?
                            image.color with { a = 1 } :
                            image.color with { a = 0.27f };
                    }
                }
            }));

            var solidBackgroundTitle = new GameObject("Title");
            solidBackgroundTitle.transform.SetParent(solidBackground.transform);
            var solidBackgroundTitleRect = solidBackgroundTitle.AddComponent<RectTransform>();
            solidBackgroundTitleRect.anchoredPosition = new Vector2(70, 45);
            solidBackgroundTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            solidBackgroundTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            solidBackgroundTitleRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundTitleRect.localScale = Vector3.one;
            var solidBackgroundTitleText = solidBackgroundTitle.AddComponent<TextMeshProUGUI>();
            solidBackgroundTitleText.text = "Solid Background:";
            solidBackgroundTitleText.fontSize = 12;
            solidBackgroundTitleText.fontStyle = FontStyles.Bold;
            solidBackgroundTitleText.color = new Color(1, 1, 1, 1);
            solidBackgroundTitleText.font = _robotoBold!;

            var solidBackgroundCheck = new GameObject("Checkbox");
            solidBackgroundCheck.transform.SetParent(solidBackground.transform);
            var solidBackgroundCheckRect = solidBackgroundCheck.AddComponent<RectTransform>();
            solidBackgroundCheckRect.anchoredPosition = new Vector2(85, 3);
            solidBackgroundCheckRect.anchorMax = Vector2.one;
            solidBackgroundCheckRect.anchorMin = Vector2.one;
            solidBackgroundCheckRect.pivot = Vector2.zero;
            solidBackgroundCheckRect.sizeDelta = new Vector2(16, 16);
            solidBackgroundCheckRect.localScale = Vector3.one;
            var solidBackgroundCheckBackground = solidBackgroundCheck.AddComponent<Image>();
            var solidBackgroundCheckBackgroundTexture = Global.LoadImageToTexture2d("CheckBox_Frame.png");
            solidBackgroundCheckBackgroundTexture.filterMode = FilterMode.Point;
            solidBackgroundCheckBackground.sprite = Sprite.Create(solidBackgroundCheckBackgroundTexture,
                new Rect(0, 0, solidBackgroundCheckBackgroundTexture.width, solidBackgroundCheckBackgroundTexture.height),
                new Vector2(0.5f, 0.5f));

            var solidBackgroundCheckmark = new GameObject("Checkmark");
            solidBackgroundCheckmark.transform.SetParent(solidBackgroundCheck.transform);
            var solidBackgroundCheckmarkRect = solidBackgroundCheckmark.AddComponent<RectTransform>();
            solidBackgroundCheckmarkRect.anchoredPosition = Vector2.zero;
            solidBackgroundCheckmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            solidBackgroundCheckmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            solidBackgroundCheckmarkRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundCheckmarkRect.sizeDelta = new Vector2(16, 16);
            solidBackgroundCheckmarkRect.localScale = Vector3.one;
            var solidBackgroundCheckmarkImage = solidBackgroundCheckmark.AddComponent<Image>();
            var solidBackgroundCheckmarkImageTexture = Global.LoadImageToTexture2d("CheckBox_Tick.png");
            solidBackgroundCheckmarkImageTexture.filterMode = FilterMode.Point;
            solidBackgroundCheckmarkImage.sprite = Sprite.Create(solidBackgroundCheckmarkImageTexture,
                new Rect(0, 0, solidBackgroundCheckmarkImageTexture.width, solidBackgroundCheckmarkImageTexture.height),
                new Vector2(0.5f, 0.5f));

            solidBackgroundToggle.graphic = solidBackgroundCheckmarkImage;
            solidBackgroundToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        }

        private static void SetupSolidBackgroundOpacity(GameObject attachTo)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var solidBackgroundOpacityGroup = new GameObject("Solid Background Opacity");
            solidBackgroundOpacityGroup.transform.SetParent(attachTo.transform);
            var solidBackgroundOpacityGroupRect = solidBackgroundOpacityGroup.AddComponent<RectTransform>();
            solidBackgroundOpacityGroupRect.anchoredPosition = new Vector2(0, -155);
            solidBackgroundOpacityGroupRect.anchorMax = Vector2.up;
            solidBackgroundOpacityGroupRect.anchorMin = Vector2.up;
            solidBackgroundOpacityGroupRect.pivot = Vector2.up;
            solidBackgroundOpacityGroupRect.localScale = Vector3.one;

            var solidBackgroundOpacityTitle = new GameObject("Title");
            solidBackgroundOpacityTitle.transform.SetParent(solidBackgroundOpacityGroup.transform);
            var solidBackgroundOpacityTitleRect = solidBackgroundOpacityTitle.AddComponent<RectTransform>();
            solidBackgroundOpacityTitleRect.anchoredPosition = new Vector2(70, 45);
            solidBackgroundOpacityTitleRect.anchorMax = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacityTitleRect.anchorMin = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacityTitleRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacityTitleRect.localScale = Vector3.one;
            var solidBackgroundOpacityTitleText = solidBackgroundOpacityTitle.AddComponent<TextMeshProUGUI>();
            solidBackgroundOpacityTitleText.text = $"Solid Background Opacity: {characterQuests.SolidBackgroundOpacity}";
            solidBackgroundOpacityTitleText.fontSize = 12;
            solidBackgroundOpacityTitleText.fontStyle = FontStyles.Bold;
            solidBackgroundOpacityTitleText.color = new Color(1, 1, 1, 1);
            solidBackgroundOpacityTitleText.font = _robotoBold!;

            var solidBackgroundOpacity = new GameObject("Solid Background Opacity Slider");
            solidBackgroundOpacity.transform.SetParent(solidBackgroundOpacityGroup.transform);
            var solidBackgroundOpacitySliderRect = solidBackgroundOpacity.AddComponent<RectTransform>();
            solidBackgroundOpacitySliderRect.anchoredPosition = new Vector2(70, 45);
            solidBackgroundOpacitySliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderRect.localScale = Vector3.one;
            solidBackgroundOpacitySliderRect.sizeDelta = new Vector2(200, 20);
            var solidBackgroundSlider = solidBackgroundOpacity.AddComponent<Slider>();
            solidBackgroundSlider.maxValue = 100;
            solidBackgroundSlider.minValue = 0;
            solidBackgroundSlider.normalizedValue = 0.7f;
            solidBackgroundSlider.value = characterQuests.SolidBackgroundOpacity;
            solidBackgroundSlider.wholeNumbers = true;
            solidBackgroundSlider.onValueChanged.AddListener(new Action<float>(_ => SolidBackgroundOpacityChanged(solidBackgroundSlider, solidBackgroundOpacityTitleText)));

            var solidBackgroundOpacitySliderBackground = new GameObject("Background");
            solidBackgroundOpacitySliderBackground.transform.SetParent(solidBackgroundOpacity.transform);
            var solidBackgroundOpacitySliderBackgroundRect = solidBackgroundOpacitySliderBackground.AddComponent<RectTransform>();
            solidBackgroundOpacitySliderBackgroundRect.anchoredPosition = Vector2.zero;
            solidBackgroundOpacitySliderBackgroundRect.anchorMax = new Vector2(1, 0.75f);
            solidBackgroundOpacitySliderBackgroundRect.anchorMin = new Vector2(0, 0.25f);
            solidBackgroundOpacitySliderBackgroundRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderBackgroundRect.localScale = Vector3.one;
            solidBackgroundOpacitySliderBackgroundRect.sizeDelta = Vector2.zero;
            var solidBackgroundOpacitySliderBackgroundImage = solidBackgroundOpacitySliderBackground.AddComponent<Image>();
            solidBackgroundOpacitySliderBackgroundImage.color = new Color(1, 1, 1, 1);

            var solidBackgroundOpacitySliderFillArea = new GameObject("Fill Area");
            solidBackgroundOpacitySliderFillArea.transform.SetParent(solidBackgroundOpacity.transform);
            var solidBackgroundOpacitySliderFillAreaRect = solidBackgroundOpacitySliderFillArea.AddComponent<RectTransform>();
            solidBackgroundOpacitySliderFillAreaRect.anchoredPosition = new Vector2(-5, 0);
            solidBackgroundOpacitySliderFillAreaRect.anchorMax = new Vector2(1, 0.75f);
            solidBackgroundOpacitySliderFillAreaRect.anchorMin = new Vector2(0, 0.25f);
            solidBackgroundOpacitySliderFillAreaRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderFillAreaRect.localScale = Vector3.one;
            solidBackgroundOpacitySliderFillAreaRect.sizeDelta = new Vector2(-20, 0);
            var solidBackgroundOpacitySliderFillAreaFill = new GameObject("Fill");
            solidBackgroundOpacitySliderFillAreaFill.transform.SetParent(solidBackgroundOpacitySliderFillArea.transform);
            var solidBackgroundOpacitySliderFillAreaFillRect = solidBackgroundOpacitySliderFillAreaFill.AddComponent<RectTransform>();
            solidBackgroundOpacitySliderFillAreaFillRect.anchoredPosition = Vector2.zero;
            solidBackgroundOpacitySliderFillAreaFillRect.anchorMax = new Vector2(0.51f, 1);
            solidBackgroundOpacitySliderFillAreaFillRect.anchorMin = Vector2.zero;
            solidBackgroundOpacitySliderFillAreaFillRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderFillAreaFillRect.localScale = Vector3.one;
            solidBackgroundOpacitySliderFillAreaFillRect.sizeDelta = new Vector2(10, 0);
            var solidBackgroundOpacitySliderFillAreaFillImage = solidBackgroundOpacitySliderFillAreaFill.AddComponent<Image>();
            solidBackgroundOpacitySliderFillAreaFillImage.color = new Color(1, 1, 1, 1);
            solidBackgroundSlider.fillRect = solidBackgroundOpacitySliderFillAreaFillRect;

            var solidBackgroundOpacitySliderHandle = new GameObject("Handle");
            solidBackgroundOpacitySliderHandle.transform.SetParent(solidBackgroundOpacity.transform);
            var solidBackgroundOpacitySliderHandleRect = solidBackgroundOpacitySliderHandle.AddComponent<RectTransform>();
            solidBackgroundOpacitySliderHandleRect.anchoredPosition = Vector2.zero;
            solidBackgroundOpacitySliderHandleRect.anchorMax = Vector2.one;
            solidBackgroundOpacitySliderHandleRect.anchorMin = Vector2.zero;
            solidBackgroundOpacitySliderHandleRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderHandleRect.localScale = Vector3.one;
            solidBackgroundOpacitySliderHandleRect.sizeDelta = new Vector2(-20, 0);
            var solidBackgroundOpacitySliderHandleHandle = new GameObject("Handle");
            solidBackgroundOpacitySliderHandleHandle.transform.SetParent(solidBackgroundOpacitySliderHandle.transform);
            var solidBackgroundOpacitySliderHandleHandleRect = solidBackgroundOpacitySliderHandleHandle.AddComponent<RectTransform>();
            solidBackgroundOpacitySliderHandleHandleRect.anchoredPosition = Vector2.zero;
            solidBackgroundOpacitySliderHandleHandleRect.anchorMax = new Vector2(0.51f, 1);
            solidBackgroundOpacitySliderHandleHandleRect.anchorMin = new Vector2(0.51f, 0);
            solidBackgroundOpacitySliderHandleHandleRect.pivot = new Vector2(0.5f, 0.5f);
            solidBackgroundOpacitySliderHandleHandleRect.sizeDelta = new Vector2(10, 0);
            solidBackgroundOpacitySliderHandleHandleRect.localScale = Vector3.one;
            var solidBackgroundOpacitySliderHandleHandleImage = solidBackgroundOpacitySliderHandleHandle.AddComponent<Image>();
            var solidBackgroundOpacitySliderHandleHandleTexture = Global.LoadImageToTexture2d("ScrollBarVertical.png");
            solidBackgroundOpacitySliderHandleHandleImage.sprite = Sprite.Create(solidBackgroundOpacitySliderHandleHandleTexture,
                new Rect(0, 0, solidBackgroundOpacitySliderHandleHandleTexture.width, solidBackgroundOpacitySliderHandleHandleTexture.height),
                new Vector2(0.5f, 0.5f));
            solidBackgroundSlider.handleRect = solidBackgroundOpacitySliderHandleHandleRect;
            solidBackgroundSlider.image = solidBackgroundOpacitySliderHandleHandleImage;
            solidBackgroundSlider.targetGraphic = solidBackgroundOpacitySliderHandleHandleImage;
        }

        private static void SolidBackgroundOpacityChanged(Slider slider, TextMeshProUGUI text)
        {
            if (Global.TrackerModCategory == null)
                return;

            var characterQuests = Global.TrackerModCategory
                .GetEntry<PlayerModQuest>(Global.TrackerModCategoryPlayersQuests).Value;

            var sliderValue = slider.value;
            text.text = $"Solid Background Opacity: {sliderValue}";
            characterQuests.SolidBackgroundOpacity = sliderValue;

            if (Global.QuestTrackerGameObject == null || !characterQuests.SolidBackground)
                return;

            var backgroundOpacity = Global.QuestTrackerGameObject
                .GetComponentsInChildren<ImageOpacityHandler>()
                .FirstOrDefault(op => op.MainBackground);

            if (backgroundOpacity == null)
                return;

            var solidBackground = backgroundOpacity.gameObject.GetComponent<Image>();

            if (solidBackground != null)
                solidBackground.color = solidBackground.color with { a = sliderValue / 100 };
        }

        #endregion Private Methods
    }
}