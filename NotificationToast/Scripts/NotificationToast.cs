using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.NotificationToast
{
    /// <summary>
    /// Queue-based toast notification system with severity-driven styling and slide animations.
    /// Assign the toast UI elements in the Inspector, position the panel where it should appear
    /// when visible, then call <see cref="Show(string, ToastSeverity)"/> from anywhere via the
    /// static <see cref="Instance"/>. Notifications display one at a time; extras are queued
    /// and shown in order.
    /// <para>
    /// The panel's anchored position in the editor is treated as the "visible" position.
    /// On Awake the panel slides off-screen by <see cref="_slideDistance"/> in the chosen
    /// <see cref="_slideDirection"/> and waits for the first notification.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Notification Toast")]
    public class NotificationToast : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("The RectTransform of the toast panel that slides in and out.")]
        [SerializeField] private RectTransform _toastPanel;

        [Tooltip("CanvasGroup on the toast panel — drives alpha fade during transitions.")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Tooltip("Background Image of the toast — tinted per severity.")]
        [SerializeField] private Image _background;

        [Tooltip("Optional icon Image — sprite and tint are set per severity.")]
        [SerializeField] private Image _iconImage;

        [Tooltip("TextMeshProUGUI label that displays the notification message.")]
        [SerializeField] private TextMeshProUGUI _messageText;

        [Header("Timing")]
        [Tooltip("How long the toast stays fully visible before fading out (seconds).")]
        [SerializeField] private float _displayDuration = 3f;

        [Tooltip("Duration of the slide-in and fade-in transition (seconds).")]
        [SerializeField] private float _fadeInDuration = 0.3f;

        [Tooltip("Duration of the slide-out and fade-out transition (seconds).")]
        [SerializeField] private float _fadeOutDuration = 0.3f;

        [Header("Animation")]
        [Tooltip("Direction from which the toast slides into view.")]
        [SerializeField] private SlideDirection _slideDirection = SlideDirection.Top;

        [Tooltip("Distance in pixels the toast travels during the slide animation.")]
        [SerializeField] private float _slideDistance = 100f;

        [Tooltip("Use unscaled time so toasts animate while the game is paused.")]
        [SerializeField] private bool _useUnscaledTime = true;

        [Header("Queue")]
        [Tooltip("Maximum number of pending notifications. Excess notifications are dropped.")]
        [SerializeField] private int _maxQueueSize = 10;

        [Header("Severity Styles")]
        [SerializeField] private SeverityStyle _infoStyle = new SeverityStyle
        {
            backgroundColor = new Color(0.2f, 0.6f, 1f, 0.9f),
            textColor = Color.white
        };

        [SerializeField] private SeverityStyle _successStyle = new SeverityStyle
        {
            backgroundColor = new Color(0.2f, 0.8f, 0.3f, 0.9f),
            textColor = Color.white
        };

        [SerializeField] private SeverityStyle _warningStyle = new SeverityStyle
        {
            backgroundColor = new Color(1f, 0.75f, 0.1f, 0.9f),
            textColor = Color.black
        };

        [SerializeField] private SeverityStyle _errorStyle = new SeverityStyle
        {
            backgroundColor = new Color(0.9f, 0.2f, 0.2f, 0.9f),
            textColor = Color.white
        };

        [Header("Events")]
        [Tooltip("Fired when a toast becomes visible. Passes the message string.")]
        public UnityEvent<string> OnToastShown;

        [Tooltip("Fired when a toast finishes dismissing. Passes the message string.")]
        public UnityEvent<string> OnToastDismissed;

        /// <summary>Most recently created instance, for convenient global access.</summary>
        public static NotificationToast Instance { get; private set; }

        /// <summary>True while a toast is visible (including during transitions).</summary>
        public bool IsShowing => _state != ToastState.Idle;

        /// <summary>Number of notifications waiting in the queue.</summary>
        public int QueueCount => _queue.Count;

        /// <summary>The message currently being displayed, or null if idle.</summary>
        public string CurrentMessage => _state != ToastState.Idle ? _current.Message : null;

        private enum ToastState { Idle, FadingIn, Displaying, FadingOut }

        private readonly Queue<ToastData> _queue = new Queue<ToastData>();
        private ToastState _state = ToastState.Idle;
        private float _timer;
        private ToastData _current;
        private Vector2 _restPosition;
        private Vector2 _hiddenPosition;

        private void Awake()
        {
            Instance = this;

            if (_toastPanel != null)
            {
                _restPosition = _toastPanel.anchoredPosition;
                _hiddenPosition = _restPosition + GetSlideOffset();
                _toastPanel.anchoredPosition = _hiddenPosition;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Shows a notification with the given message and severity, using the Inspector's
        /// default display duration. If a toast is already visible the new one is queued.
        /// </summary>
        /// <param name="message">Text to display in the toast.</param>
        /// <param name="severity">Severity level that controls background color, text color, and icon.</param>
        public void Show(string message, ToastSeverity severity = ToastSeverity.Info)
        {
            Show(message, severity, -1f);
        }

        /// <summary>
        /// Shows a notification with the given message, severity, and custom display duration.
        /// If a toast is already visible the new one is queued.
        /// </summary>
        /// <param name="message">Text to display in the toast.</param>
        /// <param name="severity">Severity level that controls styling.</param>
        /// <param name="duration">Display duration in seconds. Pass a negative value to use the Inspector default.</param>
        public void Show(string message, ToastSeverity severity, float duration)
        {
            if (_toastPanel == null || _messageText == null)
            {
                Debug.LogWarning("NotificationToast: missing UI references. " +
                                 "Assign the toast panel and message text in the Inspector.", this);
                return;
            }

            var data = new ToastData
            {
                Message = message,
                Severity = severity,
                Duration = duration < 0f ? _displayDuration : duration
            };

            if (_state != ToastState.Idle)
            {
                if (_queue.Count >= _maxQueueSize)
                {
                    Debug.LogWarning("NotificationToast: queue is full, dropping notification.", this);
                    return;
                }

                _queue.Enqueue(data);
                return;
            }

            BeginShow(data);
        }

        /// <summary>
        /// Dismisses the current toast immediately and clears the entire queue.
        /// </summary>
        public void DismissAll()
        {
            _queue.Clear();

            if (_state == ToastState.Idle)
                return;

            string msg = _current.Message;
            FinishDismiss();
            OnToastDismissed.Invoke(msg);
        }

        /// <summary>
        /// Dismisses the current toast immediately and advances to the next queued notification.
        /// </summary>
        public void DismissCurrent()
        {
            if (_state == ToastState.Idle)
                return;

            string msg = _current.Message;
            FinishDismiss();
            OnToastDismissed.Invoke(msg);
            TryShowNext();
        }

        /// <summary>
        /// Clears all queued notifications without affecting the currently displayed toast.
        /// </summary>
        public void ClearQueue()
        {
            _queue.Clear();
        }

        private void Update()
        {
            if (_state == ToastState.Idle)
                return;

            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _timer += dt;

            switch (_state)
            {
                case ToastState.FadingIn:
                    UpdateFadeIn();
                    break;
                case ToastState.Displaying:
                    UpdateDisplay();
                    break;
                case ToastState.FadingOut:
                    UpdateFadeOut();
                    break;
            }
        }

        private void BeginShow(ToastData data)
        {
            _current = data;
            _timer = 0f;

            ApplyStyle(GetStyle(data.Severity));
            _messageText.text = data.Message;

            if (_toastPanel != null)
                _toastPanel.anchoredPosition = _hiddenPosition;

            if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;

            _state = ToastState.FadingIn;
            OnToastShown.Invoke(data.Message);
        }

        private void UpdateFadeIn()
        {
            float t = Mathf.Clamp01(_timer / Mathf.Max(_fadeInDuration, 0.001f));
            float eased = EaseOutCubic(t);

            if (_toastPanel != null)
                _toastPanel.anchoredPosition = Vector2.Lerp(_hiddenPosition, _restPosition, eased);

            if (_canvasGroup != null)
                _canvasGroup.alpha = eased;

            if (t >= 1f)
            {
                _state = ToastState.Displaying;
                _timer = 0f;
            }
        }

        private void UpdateDisplay()
        {
            if (_timer >= _current.Duration)
            {
                _state = ToastState.FadingOut;
                _timer = 0f;
            }
        }

        private void UpdateFadeOut()
        {
            float t = Mathf.Clamp01(_timer / Mathf.Max(_fadeOutDuration, 0.001f));
            float eased = EaseInCubic(t);

            if (_toastPanel != null)
                _toastPanel.anchoredPosition = Vector2.Lerp(_restPosition, _hiddenPosition, eased);

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f - eased;

            if (t >= 1f)
            {
                string msg = _current.Message;
                FinishDismiss();
                OnToastDismissed.Invoke(msg);
                TryShowNext();
            }
        }

        private void FinishDismiss()
        {
            _state = ToastState.Idle;

            if (_toastPanel != null)
                _toastPanel.anchoredPosition = _hiddenPosition;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
        }

        private void TryShowNext()
        {
            if (_queue.Count > 0)
                BeginShow(_queue.Dequeue());
        }

        private SeverityStyle GetStyle(ToastSeverity severity)
        {
            return severity switch
            {
                ToastSeverity.Success => _successStyle,
                ToastSeverity.Warning => _warningStyle,
                ToastSeverity.Error => _errorStyle,
                _ => _infoStyle,
            };
        }

        private void ApplyStyle(SeverityStyle style)
        {
            if (_background != null)
                _background.color = style.backgroundColor;

            if (_messageText != null)
                _messageText.color = style.textColor;

            if (_iconImage != null)
            {
                if (style.icon != null)
                {
                    _iconImage.sprite = style.icon;
                    _iconImage.color = style.textColor;
                    _iconImage.enabled = true;
                }
                else
                {
                    _iconImage.enabled = false;
                }
            }
        }

        private Vector2 GetSlideOffset()
        {
            return _slideDirection switch
            {
                SlideDirection.Top => new Vector2(0f, _slideDistance),
                SlideDirection.Bottom => new Vector2(0f, -_slideDistance),
                SlideDirection.Left => new Vector2(-_slideDistance, 0f),
                SlideDirection.Right => new Vector2(_slideDistance, 0f),
                _ => new Vector2(0f, _slideDistance),
            };
        }

        private static float EaseOutCubic(float t)
        {
            float inv = 1f - t;
            return 1f - inv * inv * inv;
        }

        private static float EaseInCubic(float t)
        {
            return t * t * t;
        }
    }

    /// <summary>
    /// Severity level for toast notifications, controlling visual styling.
    /// </summary>
    public enum ToastSeverity
    {
        /// <summary>General information — neutral blue styling.</summary>
        Info,

        /// <summary>Positive outcome — green styling.</summary>
        Success,

        /// <summary>Non-critical alert — yellow/amber styling.</summary>
        Warning,

        /// <summary>Critical failure — red styling.</summary>
        Error
    }

    /// <summary>
    /// Direction from which the toast panel slides into view.
    /// </summary>
    public enum SlideDirection
    {
        /// <summary>Slides down from above the rest position.</summary>
        Top,

        /// <summary>Slides up from below the rest position.</summary>
        Bottom,

        /// <summary>Slides right from the left of the rest position.</summary>
        Left,

        /// <summary>Slides left from the right of the rest position.</summary>
        Right
    }

    /// <summary>
    /// Per-severity visual settings for toast notifications.
    /// Assign colors and an optional icon sprite for each severity level.
    /// </summary>
    [System.Serializable]
    public class SeverityStyle
    {
        [Tooltip("Background color of the toast panel for this severity.")]
        public Color backgroundColor = Color.white;

        [Tooltip("Text and icon tint color for this severity.")]
        public Color textColor = Color.black;

        [Tooltip("Optional icon sprite shown beside the message.")]
        public Sprite icon;
    }

    internal struct ToastData
    {
        public string Message;
        public ToastSeverity Severity;
        public float Duration;
    }
}
