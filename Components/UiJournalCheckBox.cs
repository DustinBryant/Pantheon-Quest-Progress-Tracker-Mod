using MelonLoader;
using PantheonQuestProgressTrackerMod.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace PantheonQuestProgressTrackerMod.Components
{
    [RegisterTypeInIl2Cpp]
    internal class UiJournalCheckBox : MonoBehaviour
    {
        #region Private Fields

        private Button? _checkBoxButton;
        private Image? _checkMarkImage;
        private int _questId;

        #endregion Private Fields

        #region Public Methods

        public void Initialize(int questId)
        {
            _questId = questId;

            var questTrackerCheckBox = gameObject;

            var checkBoxRect = questTrackerCheckBox.AddComponent<RectTransform>();
            checkBoxRect.anchoredPosition = new Vector2(-25, -13.5f);
            checkBoxRect.anchorMax = new Vector2(0, 1);
            checkBoxRect.anchorMin = new Vector2(0, 1);
            checkBoxRect.sizeDelta = new Vector2(15, 15);
            checkBoxRect.localScale = Vector3.one;
            var checkBoxImage = questTrackerCheckBox.AddComponent<Image>();
            _checkBoxButton = questTrackerCheckBox.AddComponent<Button>();
            var checkBoxBackgroundTexture = Global.LoadImageToTexture2d("GenericCheckboxBkg.png");
            checkBoxImage.sprite = Sprite.Create(checkBoxBackgroundTexture, new Rect(0, 0, checkBoxBackgroundTexture.width, checkBoxBackgroundTexture.height), new Vector2(0.5f, 0.5f));
            _checkBoxButton.onClick.AddListener(new Action(CheckBoxClick));

            var checkMark = new GameObject("CheckMark");
            checkMark.transform.SetParent(questTrackerCheckBox.transform);
            var checkMarkRect = checkMark.AddComponent<RectTransform>();
            checkMarkRect.anchoredPosition = Vector2.zero;
            checkMarkRect.sizeDelta = new Vector2(9, 9);
            checkMarkRect.localScale = Vector3.one;
            _checkMarkImage = checkMark.AddComponent<Image>();
            var checkBoxCheckTexture = Global.LoadImageToTexture2d("GenericCheckBoxTick.png");
            _checkMarkImage.sprite = Sprite.Create(checkBoxCheckTexture, new Rect(0, 0, checkBoxCheckTexture.width, checkBoxCheckTexture.height), new Vector2(0.5f, 0.5f));
            _checkMarkImage.enabled = Global.IsQuestTracked(_questId);
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckBoxClick()
        {
            if (_checkMarkImage == null)
                return;

            _checkMarkImage.enabled = !_checkMarkImage.enabled;

            if (_checkMarkImage.enabled)
                TrackerPanel.AddQuest(_questId);
            else
                TrackerPanel.RemoveQuest(_questId);
        }

        private void OnDestroy()
        {
            _checkBoxButton?.onClick.RemoveAllListeners();
        }

        public void RecheckIfChecked()
        {
            if (_checkMarkImage == null)
                return;

            _checkMarkImage.enabled = Global.IsQuestTracked(_questId);
        }

        #endregion Private Methods
    }
}