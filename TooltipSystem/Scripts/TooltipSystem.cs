using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.TooltipSystem
{
    /// <summary>
    /// Manages a single tooltip panel that repositions itself to stay within the screen bounds.
    /// Attach to any GameObject, assign the UI references, and call <see cref="Show(string, Vector2)"/>
    /// or use <see cref="TooltipTrigger"/> components on individual UI elements.
    /// <para>
    /// The tooltip automatically pivots around the cursor/target position so it never clips
    /// off-screen. A configurable offset keeps it from overlapping the pointer.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Tooltip System")]
    public class TooltipSystem : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("The RectTransform of the tooltip panel.")]
        [SerializeField] private RectTransform _tooltipPanel;

        [Tooltip("CanvasGroup on the tooltip panel — drives alpha fade.")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Tooltip("Optional background Image for the tooltip.")]
        [SerializeField] private Image _background;

        [Tooltip("TextMeshProUGUI label that displays the tooltip text.")]
        [SerializeField] private TextMeshProUGUI _messageText;

        [Header("Positioning")]
        [Tooltip("Pixel offset from the pointer/target position.")]
        [SerializeField] private Vector2 _offset = new Vector2(16f, -16f);

        [Tooltip("Minimum distance in pixels from any screen edge.")]
        [SerializeField] private float _screenPadding = 8f;

        [Tooltip("The Canvas this tooltip lives on. Auto-detected from the panel if left empty.")]
        [SerializeField] private Canvas _canvas;

        [Header("Animation")]
        [Tooltip("Duration of the fade-in transition (seconds).")]
        [SerializeField] private float _fadeInDuration = 0.15f;

        [Tooltip("Duration of the fade-out transition (seconds).")]
        [SerializeField] private float _fadeOutDuration = 0.1f;

        [Tooltip("Use unscaled time so tooltips work while the game is paused.")]
        [SerializeField] private bool _useUnscaledTime = true;

        [Header("Delay")]
        [Tooltip("Seconds the pointer must hover before the tooltip appears.")]
        [SerializeField] private float _showDelay = 0.4f;

        [Header("Styling")]
        [Tooltip("Default background color.")]
        [SerializeField] private Color _backgroundColor = new Color(0.12f, 0.12f, 0.12f, 0.92f);

        [Tooltip("Default text color.")]
        [SerializeField] private Color _textColor = Color.white;

        [Header("Events")]
        [Tooltip("Fired when the tooltip becomes visible.")]
        public UnityEvent<string> OnTooltipShown;

        [Tooltip("Fired when the tooltip is hidden.")]
        public UnityEvent OnTooltipHidden;

        /// <summary>Most recently created instance, for convenient global access.</summary>
        public static TooltipSystem Instance { get; private set; }

        /// <summary>True while the tooltip is visible or transitioning.</summary>
        public bool IsVisible => _state != TooltipState.Hidden;

        /// <summary>The text currently displayed, or null if hidden.</summary>
        public string CurrentText => _state != TooltipState.Hidden ? _currentText : null;

        private enum TooltipState { Hidden, Waiting, FadingIn, Visible, FadingOut }

        private TooltipState _state = TooltipState.Hidden;
        private float _timer;
        private string _currentText;
        private Vector2 _targetScreenPosition;
        private bool _followPointer;
        private RectTransform _canvasRect;

        private void Awake()
        {
            Instance = this;

            if (_canvas == null && _tooltipPanel != null)
                _canvas = _tooltipPanel.GetComponentInParent<Canvas>();

            if (_canvas != null)
                _canvasRect = _canvas.GetComponent<RectTransform>();

            SetPanelVisible(false);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Shows the tooltip at the current pointer position and follows it until hidden.
        /// The tooltip appears after the configured <see cref="_showDelay"/>.
        /// </summary>
        /// <param name="text">Text to display in the tooltip.</param>
        public void Show(string text)
        {
            Show(text, GetPointerScreenPosition(), true);
        }

        /// <summary>
        /// Shows the tooltip at a fixed screen position. It does not follow the pointer.
        /// </summary>
        /// <param name="text">Text to display in the tooltip.</param>
        /// <param name="screenPosition">Screen-space position to anchor the tooltip.</param>
        public void Show(string text, Vector2 screenPosition)
        {
            Show(text, screenPosition, false);
        }

        /// <summary>
        /// Shows the tooltip at the given screen position.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="screenPosition">Screen-space anchor position.</param>
        /// <param name="followPointer">If true, the tooltip follows the pointer each frame.</param>
        public void Show(string text, Vector2 screenPosition, bool followPointer)
        {
            if (_tooltipPanel == null || _messageText == null)
            {
                Debug.LogWarning("TooltipSystem: missing UI references. " +
                                 "Assign the tooltip panel and message text in the Inspector.", this);
                return;
            }

            _currentText = text;
            _targetScreenPosition = screenPosition;
            _followPointer = followPointer;
            _timer = 0f;

            _messageText.text = text;
            ApplyStyle();

            LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipPanel);

            if (_showDelay <= 0f)
            {
                BeginFadeIn();
            }
            else
            {
                _state = TooltipState.Waiting;
                SetPanelVisible(false);
            }
        }

        /// <summary>
        /// Hides the tooltip with a fade-out transition.
        /// </summary>
        public void Hide()
        {
            if (_state == TooltipState.Hidden || _state == TooltipState.FadingOut)
                return;

            _timer = 0f;
            _state = TooltipState.FadingOut;
        }

        /// <summary>
        /// Hides the tooltip immediately without any transition.
        /// </summary>
        public void HideImmediate()
        {
            if (_state == TooltipState.Hidden)
                return;

            SetPanelVisible(false);
            _state = TooltipState.Hidden;
            OnTooltipHidden.Invoke();
        }

        private void Update()
        {
            if (_state == TooltipState.Hidden)
                return;

            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _timer += dt;

            if (_followPointer && _state != TooltipState.FadingOut)
                _targetScreenPosition = GetPointerScreenPosition();

            switch (_state)
            {
                case TooltipState.Waiting:
                    UpdateWaiting();
                    break;
                case TooltipState.FadingIn:
                    UpdateFadeIn();
                    break;
                case TooltipState.Visible:
                    UpdatePosition();
                    break;
                case TooltipState.FadingOut:
                    UpdateFadeOut();
                    break;
            }
        }

        private void UpdateWaiting()
        {
            if (_timer >= _showDelay)
                BeginFadeIn();
        }

        private void BeginFadeIn()
        {
            _timer = 0f;
            _state = TooltipState.FadingIn;

            if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;

            _tooltipPanel.gameObject.SetActive(true);
            UpdatePosition();
            OnTooltipShown.Invoke(_currentText);
        }

        private void UpdateFadeIn()
        {
            float t = Mathf.Clamp01(_timer / Mathf.Max(_fadeInDuration, 0.001f));

            if (_canvasGroup != null)
                _canvasGroup.alpha = t;

            UpdatePosition();

            if (t >= 1f)
            {
                _state = TooltipState.Visible;

                if (_canvasGroup != null)
                    _canvasGroup.alpha = 1f;
            }
        }

        private void UpdateFadeOut()
        {
            float t = Mathf.Clamp01(_timer / Mathf.Max(_fadeOutDuration, 0.001f));

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f - t;

            if (t >= 1f)
            {
                SetPanelVisible(false);
                _state = TooltipState.Hidden;
                OnTooltipHidden.Invoke();
            }
        }

        private void UpdatePosition()
        {
            if (_tooltipPanel == null || _canvasRect == null)
                return;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, _targetScreenPosition + _offset, GetCamera(), out localPoint);

            Vector2 tooltipSize = _tooltipPanel.sizeDelta;
            Vector2 canvasSize = _canvasRect.sizeDelta;

            Vector2 pivotOffset = new Vector2(
                tooltipSize.x * _tooltipPanel.pivot.x,
                tooltipSize.y * _tooltipPanel.pivot.y);

            float scaledPadding = _screenPadding;
            if (_canvas != null && _canvas.scaleFactor > 0f)
                scaledPadding = _screenPadding / _canvas.scaleFactor;

            float halfCanvasW = canvasSize.x * 0.5f;
            float halfCanvasH = canvasSize.y * 0.5f;

            float minX = -halfCanvasW + scaledPadding + pivotOffset.x;
            float maxX = halfCanvasW - scaledPadding - (tooltipSize.x - pivotOffset.x);
            float minY = -halfCanvasH + scaledPadding + pivotOffset.y;
            float maxY = halfCanvasH - scaledPadding - (tooltipSize.y - pivotOffset.y);

            bool flippedX = false;
            bool flippedY = false;

            if (localPoint.x > maxX)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect, _targetScreenPosition - new Vector2(_offset.x, -_offset.y),
                    GetCamera(), out localPoint);
                flippedX = true;
            }

            if (localPoint.y < minY)
            {
                Vector2 flipOffset = flippedX
                    ? new Vector2(-_offset.x, -_offset.y)
                    : new Vector2(_offset.x, -_offset.y);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect, _targetScreenPosition + flipOffset, GetCamera(), out localPoint);
                flippedY = true;
            }

            localPoint.x = Mathf.Clamp(localPoint.x, minX, maxX);
            localPoint.y = Mathf.Clamp(localPoint.y, minY, maxY);

            _tooltipPanel.localPosition = localPoint;
        }

        private Camera GetCamera()
        {
            if (_canvas == null)
                return null;

            return _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        }

        private void ApplyStyle()
        {
            if (_background != null)
                _background.color = _backgroundColor;

            if (_messageText != null)
                _messageText.color = _textColor;
        }

        private void SetPanelVisible(bool visible)
        {
            if (_tooltipPanel != null)
                _tooltipPanel.gameObject.SetActive(visible);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1f : 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
        }

        private static Vector2 GetPointerScreenPosition()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
                return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.mousePosition;
#else
            return Vector2.zero;
#endif
        }
    }
}
