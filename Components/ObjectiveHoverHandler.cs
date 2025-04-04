using MelonLoader;
using UnityEngine;

namespace PantheonQuestProgressTrackerMod.Components
{
    [RegisterTypeInIl2Cpp]
    public class ObjectiveHoverHandler : MonoBehaviour
    {
        #region Private Fields

        private bool _isHovered;
        private RectTransform? _rectTransform;
        private GameObject? _removeButtonObject;
        private GameObject? _wwwButtonObject;

        #endregion Private Fields

        #region Private Methods

        private void Start()
        {
            _removeButtonObject = gameObject.transform.GetChild(0).gameObject;
            _wwwButtonObject = gameObject.transform.GetChild(2).gameObject;
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_removeButtonObject == null || _rectTransform == null || _wwwButtonObject == null || Global.IsResizing)
                return;

            var mouseOver = RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, Input.mousePosition);

            if (_isHovered && !mouseOver)
            {
                _isHovered = false;
                _removeButtonObject.SetActive(false);
                _wwwButtonObject.SetActive(false);
            }

            if (!_isHovered && mouseOver)
            {
                _isHovered = true;
                _removeButtonObject.SetActive(true);
                _wwwButtonObject.SetActive(true);
            }
        }

        #endregion Private Methods
    }
}