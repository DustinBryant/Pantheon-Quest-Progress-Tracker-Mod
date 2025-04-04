using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace PantheonQuestProgressTrackerMod.Components
{
    [RegisterTypeInIl2Cpp]
    public class WindowResizeHandle : MonoBehaviour
    {
        public UIDraggable? Draggable;
        public float MinHeight = 100f;
        public RectTransform? ResizeWindowRect;

        private RectTransform? _handleRect;
        private Vector2 _startMousePosition;
        private Vector2 _startPanelOffsetMin;

        private void Start()
        {
            _handleRect = GetComponent<RectTransform>();

            if (_handleRect != null)
                return;

            enabled = false;
        }

        private void EndResize()
        {
            Global.IsResizing = false;

            // Re-enable Draggable after resizing complete
            if (Draggable != null)
                Draggable.enabled = true;
        }

        private void StartResize()
        {
            if (ResizeWindowRect == null)
                return;

            // Set Draggable to disabled while resizing
            if (Draggable != null)
                Draggable.enabled = false;

            Global.IsResizing = true;
            _startMousePosition = Input.mousePosition;
            _startPanelOffsetMin = ResizeWindowRect.offsetMin;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_handleRect, Input.mousePosition))
                    StartResize();
            }
            else if (Input.GetMouseButton(0) && Global.IsResizing)
                DoResize();
            else if (Global.IsResizing)
                EndResize();
        }

        private void DoResize()
        {
            if (ResizeWindowRect == null)
                return;

            var currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var mouseDelta = (currentMousePosition - _startMousePosition) / 2;

            // Calculate new offsetMin based on mouseDelta
            ResizeWindowRect.offsetMin = new Vector2(ResizeWindowRect.offsetMin.x, _startPanelOffsetMin.y + mouseDelta.y);

            var height = ResizeWindowRect.rect.height;
            var offsetMaxY = ResizeWindowRect.offsetMax.y;

            // If any of our dimensions are less than the minimum, adjust the offsetMax and offsetMin to match
            if (height < MinHeight)
                ResizeWindowRect.offsetMin = new Vector2(ResizeWindowRect.offsetMin.x, offsetMaxY - MinHeight);
        }
    }


}
