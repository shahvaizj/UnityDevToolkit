using UnityEngine;
using UnityEngine.EventSystems;

namespace ShahvaizJ.TooltipSystem
{
    /// <summary>
    /// Attach to any UI element with a <see cref="RectTransform"/> to show a tooltip on hover
    /// or long-press. Requires a <see cref="TooltipSystem"/> instance in the scene.
    /// <para>
    /// On desktop the tooltip appears after hovering for the system's configured delay.
    /// On touch devices (or when <see cref="_useLongPress"/> is enabled) the tooltip appears
    /// after holding for <see cref="_longPressDuration"/> seconds.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Tooltip Trigger")]
    public class TooltipTrigger : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("The text to display in the tooltip.")]
        [SerializeField] [TextArea(1, 5)] private string _tooltipText;

        [Tooltip("If true, also show the tooltip on long-press (useful for touch devices).")]
        [SerializeField] private bool _useLongPress = true;

        [Tooltip("Seconds the pointer must be held down before the tooltip appears.")]
        [SerializeField] private float _longPressDuration = 0.5f;

        [Tooltip("Maximum movement in pixels allowed during a long-press before it cancels.")]
        [SerializeField] private float _longPressTolerance = 10f;

        /// <summary>The tooltip text. Can be changed at runtime before showing.</summary>
        public string TooltipText
        {
            get => _tooltipText;
            set => _tooltipText = value;
        }

        private bool _isHovering;
        private bool _isPressed;
        private float _pressTimer;
        private Vector2 _pressStartPosition;
        private bool _longPressTriggered;

        /// <summary>Called by the EventSystem when the pointer enters this element.</summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;

            if (string.IsNullOrEmpty(_tooltipText))
                return;

            if (TooltipSystem.Instance != null)
                TooltipSystem.Instance.Show(_tooltipText);
        }

        /// <summary>Called by the EventSystem when the pointer exits this element.</summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            HideTooltip();
        }

        /// <summary>Called by the EventSystem when the pointer is pressed on this element.</summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_useLongPress)
                return;

            _isPressed = true;
            _pressTimer = 0f;
            _longPressTriggered = false;
            _pressStartPosition = eventData.position;
        }

        /// <summary>Called by the EventSystem when the pointer is released from this element.</summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;

            if (_longPressTriggered)
                HideTooltip();

            _longPressTriggered = false;
        }

        private void Update()
        {
            if (!_isPressed || _longPressTriggered || !_useLongPress)
                return;

            Vector2 currentPosition = GetPointerScreenPosition();
            if (Vector2.Distance(currentPosition, _pressStartPosition) > _longPressTolerance)
            {
                _isPressed = false;
                return;
            }

            float dt = Time.unscaledDeltaTime;
            _pressTimer += dt;

            if (_pressTimer >= _longPressDuration)
            {
                _longPressTriggered = true;

                if (!string.IsNullOrEmpty(_tooltipText) && TooltipSystem.Instance != null)
                    TooltipSystem.Instance.Show(_tooltipText, currentPosition);
            }
        }

        private void OnDisable()
        {
            _isHovering = false;
            _isPressed = false;
            _longPressTriggered = false;
            HideTooltip();
        }

        private void HideTooltip()
        {
            if (TooltipSystem.Instance != null && TooltipSystem.Instance.CurrentText == _tooltipText)
                TooltipSystem.Instance.Hide();
        }

        private static Vector2 GetPointerScreenPosition()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
                return UnityEngine.InputSystem.Mouse.current.position.ReadValue();

            if (UnityEngine.InputSystem.Touchscreen.current != null &&
                UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.isPressed)
                return UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue();
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.mousePosition;
#else
            return Vector2.zero;
#endif
        }
    }
}
