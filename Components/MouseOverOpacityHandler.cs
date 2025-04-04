using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PantheonQuestProgressTrackerMod.Components
{
    [RegisterTypeInIl2Cpp]
    public class MouseOverOpacityHandler : MonoBehaviour
    {
        #region Private Fields

        private bool _isHovered;
        private RectTransform? _rectTransform;

        #endregion Private Fields

        #region Private Methods

        private IEnumerator FadeTo(Image? image, float targetOpacity, float duration)
        {
            if (image == null)
                yield break;

            var startOpacity = image.color.a;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                var alpha = Mathf.Lerp(startOpacity, targetOpacity, time / duration);
                image.color = image.color with { a = alpha };
                yield return null;
            }

            image.color = image.color with { a = targetOpacity };
        }

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            UpdateImageOpacities();
        }

        private void Update()
        {
            if (_rectTransform == null ||
                Global.QuestTrackerGameObject?.GetComponentsInChildren<ImageOpacityHandler>().Count < 1 ||
                Global.IsResizing ||
                Global.SolidBackground)
                return;

            var mouseOver = RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, Input.mousePosition);

            if (_isHovered && !mouseOver)
            {
                _isHovered = false;
                UpdateImageOpacities();
            }

            if (!_isHovered && mouseOver)
            {
                _isHovered = true;
                UpdateImageOpacities();
            }
        }

        private void UpdateImageOpacities()
        {
            if (Global.SolidBackground || _rectTransform == null)
                return;

            if (_isHovered)
            {
                var opacityHandlers = Global.QuestTrackerGameObject?
                    .GetComponentsInChildren<ImageOpacityHandler>()
                    .Select(op => op.gameObject.GetComponent<Image>())
                    .Where(op => op != null);

                if (opacityHandlers == null)
                    return;

                // full opacity
                foreach (var image in opacityHandlers)
                    MelonCoroutines.Start(FadeTo(image, 0.87f, 0.2f));
            }
            else
            {
                var opacityHandlers = Global.QuestTrackerGameObject?
                    .GetComponentsInChildren<ImageOpacityHandler>()
                    .Select(op => op.gameObject.GetComponent<Image>())
                    .Where(op => op != null);

                if (opacityHandlers == null)
                    return;

                // 27% opacity
                foreach (var image in opacityHandlers)
                    MelonCoroutines.Start(FadeTo(image, 0.27f, 0.2f));
            }
        }

        #endregion Private Methods
    }
}