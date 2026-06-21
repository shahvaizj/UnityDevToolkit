using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.SmoothNumText
{
    /// <summary>
    /// Smoothly animates a <see cref="TMP_Text"/> label between numeric values.
    /// Drop this on any GameObject with a TextMeshPro label, then call
    /// <see cref="AnimateTo"/> from gameplay code whenever the displayed number
    /// should change — score pickups, currency transactions, health changes, etc.
    /// The component interpolates from the current value to the target over a
    /// configurable duration, shaped by an <see cref="AnimationCurve"/>.
    /// <para>
    /// Supports whole-number and decimal display, optional prefix/suffix strings
    /// (e.g. "$", " coins"), and thousands separators. Pair with any scoring or
    /// resource system for polished number feedback with zero coroutines.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Smooth Num Text")]
    public class SmoothNumText : MonoBehaviour
    {
        [Header("Text Target")]
        [Tooltip("The TextMeshPro label to display the animated number. " +
                 "Supports both TMP_Text (world-space) and TextMeshProUGUI (Canvas).")]
        [SerializeField] private TMP_Text _label;

        [Header("Number Format")]
        [Tooltip("Text prepended before the number (e.g. \"$\" or \"Score: \").")]
        [SerializeField] private string _prefix = "";

        [Tooltip("Text appended after the number (e.g. \" coins\" or \"%\").")]
        [SerializeField] private string _suffix = "";

        [Tooltip("Number of decimal places to display. Set to 0 for whole numbers.")]
        [Range(0, 4)]
        [SerializeField] private int _decimalPlaces;

        [Tooltip("Insert commas as thousands separators (e.g. 1,000,000).")]
        [SerializeField] private bool _useThousandsSeparator;

        [Header("Animation")]
        [Tooltip("Default duration in seconds for the count animation.")]
        [SerializeField] private float _defaultDuration = 0.5f;

        [Tooltip("Easing curve for the interpolation (X = normalized time, Y = progress). " +
                 "A curve that eases out makes the counter feel snappy.")]
        [SerializeField] private AnimationCurve _easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("Use unscaled time so the animation keeps running while the game is paused.")]
        [SerializeField] private bool _useUnscaledTime;

        [Header("Initial Value")]
        [Tooltip("The starting value shown on Awake. The label updates immediately without animation.")]
        [SerializeField] private float _startValue;

        [Header("Events")]
        [Tooltip("Fired when a count animation begins.")]
        public UnityEvent OnAnimationStarted;

        [Tooltip("Fired when a count animation finishes (value reached the target).")]
        public UnityEvent OnAnimationComplete;

        [Tooltip("Fired every frame during the animation with the current displayed value.")]
        public UnityEvent<float> OnValueChanged;

        /// <summary>Most recently created instance, for convenient global access.</summary>
        public static SmoothNumText Instance { get; private set; }

        /// <summary>The current displayed value (may be mid-animation).</summary>
        public float CurrentValue => _currentValue;

        /// <summary>The value the animation is heading toward.</summary>
        public float TargetValue => _targetValue;

        /// <summary>True while the counter is animating between two values.</summary>
        public bool IsAnimating => _isAnimating;

        /// <summary>Normalized progress of the current animation (0 at start, 1 at end).</summary>
        public float Progress => _progress;

        private float _currentValue;
        private float _targetValue;
        private float _fromValue;
        private float _activeDuration;
        private AnimationCurve _activeCurve;
        private float _timer;
        private float _progress;
        private bool _isAnimating;

        private void Awake()
        {
            Instance = this;
            _currentValue = _startValue;
            _targetValue = _startValue;
            RefreshLabel();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Animates the displayed number from its current value to <paramref name="target"/>
        /// using the Inspector's default duration and easing curve.
        /// </summary>
        /// <param name="target">The value to count toward.</param>
        public void AnimateTo(float target)
        {
            AnimateTo(target, _defaultDuration, _easingCurve);
        }

        /// <summary>
        /// Animates the displayed number from its current value to <paramref name="target"/>
        /// over the specified duration, using the Inspector's default easing curve.
        /// </summary>
        /// <param name="target">The value to count toward.</param>
        /// <param name="duration">How long the animation lasts, in seconds.</param>
        public void AnimateTo(float target, float duration)
        {
            AnimateTo(target, duration, _easingCurve);
        }

        /// <summary>
        /// Animates the displayed number from its current value to <paramref name="target"/>
        /// with full control over duration and easing.
        /// </summary>
        /// <param name="target">The value to count toward.</param>
        /// <param name="duration">How long the animation lasts, in seconds.</param>
        /// <param name="curve">Easing curve (Y = interpolation factor at normalized time X).</param>
        public void AnimateTo(float target, float duration, AnimationCurve curve)
        {
            if (_label == null)
            {
                Debug.LogWarning("SmoothNumText: no TMP_Text label assigned.", this);
                return;
            }

            _fromValue = _currentValue;
            _targetValue = target;
            _activeDuration = Mathf.Max(duration, 0.001f);
            _activeCurve = curve ?? _easingCurve;
            _timer = 0f;
            _progress = 0f;
            _isAnimating = true;

            OnAnimationStarted.Invoke();
        }

        /// <summary>
        /// Sets the displayed value instantly with no animation. Cancels any in-progress animation.
        /// </summary>
        /// <param name="value">The value to display immediately.</param>
        public void SetValue(float value)
        {
            _isAnimating = false;
            _currentValue = value;
            _targetValue = value;
            _progress = 1f;
            RefreshLabel();
            OnValueChanged.Invoke(_currentValue);
        }

        /// <summary>
        /// Adds <paramref name="amount"/> to the current target and animates toward the new total.
        /// Useful for incremental changes like picking up coins one at a time.
        /// </summary>
        /// <param name="amount">Amount to add (can be negative to subtract).</param>
        public void AddValue(float amount)
        {
            AnimateTo(_targetValue + amount);
        }

        /// <summary>
        /// Adds <paramref name="amount"/> to the current target and animates over the specified duration.
        /// </summary>
        /// <param name="amount">Amount to add (can be negative to subtract).</param>
        /// <param name="duration">How long the animation lasts, in seconds.</param>
        public void AddValue(float amount, float duration)
        {
            AnimateTo(_targetValue + amount, duration);
        }

        /// <summary>
        /// Cancels any in-progress animation and snaps the display to the current target value.
        /// </summary>
        public void SnapToTarget()
        {
            if (!_isAnimating)
                return;

            _isAnimating = false;
            _currentValue = _targetValue;
            _progress = 1f;
            RefreshLabel();
            OnValueChanged.Invoke(_currentValue);
            OnAnimationComplete.Invoke();
        }

        private void Update()
        {
            if (!_isAnimating || _label == null)
                return;

            float deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _timer += deltaTime;
            _progress = Mathf.Clamp01(_timer / _activeDuration);

            float easedProgress = _activeCurve.Evaluate(_progress);
            _currentValue = Mathf.LerpUnclamped(_fromValue, _targetValue, easedProgress);

            RefreshLabel();
            OnValueChanged.Invoke(_currentValue);

            if (_progress >= 1f)
            {
                _currentValue = _targetValue;
                _isAnimating = false;
                RefreshLabel();
                OnAnimationComplete.Invoke();
            }
        }

        private void RefreshLabel()
        {
            if (_label == null)
                return;

            string formatted = FormatNumber(_currentValue);
            _label.text = string.Concat(_prefix, formatted, _suffix);
        }

        private string FormatNumber(float value)
        {
            if (_decimalPlaces <= 0)
            {
                long whole = (long)Mathf.Round(value);
                return _useThousandsSeparator ? whole.ToString("N0") : whole.ToString();
            }

            string format = _useThousandsSeparator
                ? "N" + _decimalPlaces
                : "F" + _decimalPlaces;

            return value.ToString(format);
        }
    }
}
