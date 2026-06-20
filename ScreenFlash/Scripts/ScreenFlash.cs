using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.ScreenFlash
{
    /// <summary>
    /// Full-screen color flash overlay driven by an <see cref="AnimationCurve"/>.
    /// Assign a stretched UI <see cref="Image"/> that covers the entire screen, then call
    /// <see cref="Flash()"/> from gameplay code whenever something impactful happens — a hit,
    /// a pickup, a scene transition. The curve controls the alpha envelope so you can shape
    /// anything from a sharp single-frame pop to a slow fade-in/fade-out wash.
    /// <para>
    /// Pairs naturally with CameraShake: trigger both on the same event for maximum juice.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Screen Flash")]
    public class ScreenFlash : MonoBehaviour
    {
        [Header("Overlay")]
        [Tooltip("A full-screen UI Image used as the flash overlay. " +
                 "Stretch it across the Canvas and leave its alpha at 0.")]
        [SerializeField] private Image _overlay;

        [Header("Default Flash")]
        [Tooltip("Color used when Flash() is called without an explicit color.")]
        [SerializeField] private Color _defaultColor = Color.white;

        [Tooltip("Duration in seconds for the default flash.")]
        [SerializeField] private float _defaultDuration = 0.3f;

        [Tooltip("Alpha envelope over the flash lifetime (0 → 1 on the X axis). " +
                 "The Y value drives overlay alpha at each point.")]
        [SerializeField] private AnimationCurve _defaultCurve = new AnimationCurve(
            new Keyframe(0f, 1f, 0f, -2f),
            new Keyframe(1f, 0f, -2f, 0f));

        [Header("Behaviour")]
        [Tooltip("Use unscaled time so flashes still animate while the game is paused.")]
        [SerializeField] private bool _useUnscaledTime;

        [Tooltip("When a new Flash is triggered while one is already playing:\n" +
                 "• Restart — cancel the old flash and start the new one.\n" +
                 "• Ignore — keep the old flash, discard the new one.\n" +
                 "• Enqueue — play the new flash after the current one finishes.")]
        [SerializeField] private OverlapMode _overlapMode = OverlapMode.Restart;

        [Header("Events")]
        [Tooltip("Fired when a flash begins playing.")]
        public UnityEvent OnFlashStarted;

        [Tooltip("Fired when a flash finishes (alpha returns to zero).")]
        public UnityEvent OnFlashComplete;

        /// <summary>Most recently created instance, for convenient global access.</summary>
        public static ScreenFlash Instance { get; private set; }

        /// <summary>True while a flash is actively animating.</summary>
        public bool IsFlashing => _isFlashing;

        /// <summary>Normalized progress of the current flash (0 at start, 1 at end).</summary>
        public float Progress => _progress;

        /// <summary>Current alpha of the overlay image.</summary>
        public float CurrentAlpha => _overlay != null ? _overlay.color.a : 0f;

        private bool _isFlashing;
        private float _timer;
        private float _activeDuration;
        private Color _activeColor;
        private AnimationCurve _activeCurve;
        private float _progress;

        private bool _hasPending;
        private Color _pendingColor;
        private float _pendingDuration;
        private AnimationCurve _pendingCurve;

        private void Awake()
        {
            Instance = this;

            if (_overlay != null)
            {
                SetOverlayAlpha(0f);
                _overlay.raycastTarget = false;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Triggers a flash using the Inspector defaults (color, duration, curve).
        /// </summary>
        public void Flash()
        {
            Flash(_defaultColor, _defaultDuration, _defaultCurve);
        }

        /// <summary>
        /// Triggers a flash with a specific color, using the Inspector's default duration and curve.
        /// </summary>
        /// <param name="color">The flash color (alpha channel is ignored — the curve drives alpha).</param>
        public void Flash(Color color)
        {
            Flash(color, _defaultDuration, _defaultCurve);
        }

        /// <summary>
        /// Triggers a flash with a specific color and duration, using the Inspector's default curve.
        /// </summary>
        /// <param name="color">The flash color.</param>
        /// <param name="duration">How long the flash lasts, in seconds.</param>
        public void Flash(Color color, float duration)
        {
            Flash(color, duration, _defaultCurve);
        }

        /// <summary>
        /// Triggers a flash with full control over color, duration, and alpha curve.
        /// </summary>
        /// <param name="color">The flash color.</param>
        /// <param name="duration">How long the flash lasts, in seconds.</param>
        /// <param name="curve">Alpha envelope (Y = overlay alpha at normalized time X).</param>
        public void Flash(Color color, float duration, AnimationCurve curve)
        {
            if (_overlay == null)
            {
                Debug.LogWarning("ScreenFlash: no overlay Image assigned.", this);
                return;
            }

            if (_isFlashing)
            {
                switch (_overlapMode)
                {
                    case OverlapMode.Ignore:
                        return;

                    case OverlapMode.Enqueue:
                        _hasPending = true;
                        _pendingColor = color;
                        _pendingDuration = duration;
                        _pendingCurve = curve;
                        return;

                    case OverlapMode.Restart:
                    default:
                        break;
                }
            }

            StartFlash(color, duration, curve);
        }

        /// <summary>
        /// Cancels any active (and queued) flash immediately and sets the overlay alpha to zero.
        /// </summary>
        public void StopImmediate()
        {
            if (!_isFlashing)
                return;

            _hasPending = false;
            _isFlashing = false;
            _progress = 1f;
            SetOverlayAlpha(0f);
            OnFlashComplete.Invoke();
        }

        private void StartFlash(Color color, float duration, AnimationCurve curve)
        {
            _activeColor = color;
            _activeDuration = Mathf.Max(duration, 0.001f);
            _activeCurve = curve ?? _defaultCurve;
            _timer = 0f;
            _progress = 0f;
            _isFlashing = true;

            float alpha = _activeCurve.Evaluate(0f);
            ApplyColor(alpha);
            OnFlashStarted.Invoke();
        }

        private void Update()
        {
            if (!_isFlashing || _overlay == null)
                return;

            float deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _timer += deltaTime;
            _progress = Mathf.Clamp01(_timer / _activeDuration);

            float alpha = _activeCurve.Evaluate(_progress);
            ApplyColor(alpha);

            if (_progress >= 1f)
            {
                _isFlashing = false;
                SetOverlayAlpha(0f);
                OnFlashComplete.Invoke();

                if (_hasPending)
                {
                    _hasPending = false;
                    StartFlash(_pendingColor, _pendingDuration, _pendingCurve);
                }
            }
        }

        private void ApplyColor(float alpha)
        {
            _overlay.color = new Color(_activeColor.r, _activeColor.g, _activeColor.b, alpha);
        }

        private void SetOverlayAlpha(float alpha)
        {
            Color c = _overlay.color;
            c.a = alpha;
            _overlay.color = c;
        }
    }

    /// <summary>
    /// Determines what happens when <see cref="ScreenFlash.Flash"/> is called while
    /// a flash is already in progress.
    /// </summary>
    public enum OverlapMode
    {
        /// <summary>Cancel the current flash and start the new one immediately.</summary>
        Restart,

        /// <summary>Discard the new flash and let the current one finish.</summary>
        Ignore,

        /// <summary>Queue the new flash to play after the current one finishes.</summary>
        Enqueue
    }
}
