using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.RadialProgressBar
{
    /// <summary>
    /// A radial progress bar driven by Image fill amount with color gradient support.
    /// Attach to a GameObject with a Filled Image (radial type) to visualize 0–1 progress.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class RadialProgressBar : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Image _fillImage;

        [Header("Progress")]
        [SerializeField] [Range(0f, 1f)] private float _progress;
        [SerializeField] private bool _smoothTransition = true;
        [SerializeField] private float _transitionSpeed = 5f;

        [Header("Color")]
        [SerializeField] private Gradient _colorGradient = CreateDefaultGradient();
        [SerializeField] private bool _useGradientColor = true;

        [Header("Fill Settings")]
        [SerializeField] private Image.FillMethod _fillMethod = Image.FillMethod.Radial360;
        [SerializeField] private Image.Origin360 _fillOrigin360 = Image.Origin360.Top;
        [SerializeField] private bool _clockwise = true;

        [Header("Events")]
        public UnityEvent<float> OnProgressChanged;
        public UnityEvent OnFillComplete;
        public UnityEvent OnFillReset;

        /// <summary>Current target progress value (0–1).</summary>
        public float Progress
        {
            get => _progress;
            set => SetProgress(value);
        }

        /// <summary>Current displayed fill amount (may lag behind Progress when smoothing).</summary>
        public float DisplayedProgress => _displayedProgress;

        /// <summary>Whether the bar has reached 1.0.</summary>
        public bool IsComplete => _displayedProgress >= 1f;

        private float _displayedProgress;
        private bool _wasComplete;

        private void Awake()
        {
            if (_fillImage == null)
                _fillImage = GetComponent<Image>();

            ApplyFillSettings();
            _displayedProgress = _progress;
            ApplyVisuals(_displayedProgress);
        }

        private void OnValidate()
        {
            if (_fillImage == null)
                _fillImage = GetComponent<Image>();

            if (_fillImage != null)
            {
                ApplyFillSettings();
                ApplyVisuals(_progress);
            }
        }

        private void Update()
        {
            if (Mathf.Approximately(_displayedProgress, _progress))
                return;

            if (_smoothTransition)
                _displayedProgress = Mathf.MoveTowards(_displayedProgress, _progress, _transitionSpeed * Time.unscaledDeltaTime);
            else
                _displayedProgress = _progress;

            ApplyVisuals(_displayedProgress);
            OnProgressChanged.Invoke(_displayedProgress);

            if (_displayedProgress >= 1f && !_wasComplete)
            {
                _wasComplete = true;
                OnFillComplete.Invoke();
            }
            else if (_displayedProgress < 1f && _wasComplete)
            {
                _wasComplete = false;
            }
        }

        /// <summary>
        /// Set the target progress value, clamped to 0–1.
        /// </summary>
        /// <param name="value">Target progress between 0 and 1.</param>
        public void SetProgress(float value)
        {
            float clamped = Mathf.Clamp01(value);

            if (Mathf.Approximately(_progress, clamped))
                return;

            _progress = clamped;

            if (!_smoothTransition)
            {
                _displayedProgress = _progress;
                ApplyVisuals(_displayedProgress);
                OnProgressChanged.Invoke(_displayedProgress);

                if (_displayedProgress >= 1f && !_wasComplete)
                {
                    _wasComplete = true;
                    OnFillComplete.Invoke();
                }
                else if (_displayedProgress < 1f && _wasComplete)
                {
                    _wasComplete = false;
                }
            }
        }

        /// <summary>
        /// Instantly set progress without smooth transition, regardless of the smooth setting.
        /// </summary>
        /// <param name="value">Progress between 0 and 1.</param>
        public void SetProgressImmediate(float value)
        {
            float clamped = Mathf.Clamp01(value);
            _progress = clamped;
            _displayedProgress = clamped;
            ApplyVisuals(_displayedProgress);
            OnProgressChanged.Invoke(_displayedProgress);

            if (_displayedProgress >= 1f && !_wasComplete)
            {
                _wasComplete = true;
                OnFillComplete.Invoke();
            }
            else if (_displayedProgress < 1f && _wasComplete)
            {
                _wasComplete = false;
            }
        }

        /// <summary>
        /// Increment progress by a delta amount.
        /// </summary>
        /// <param name="delta">Amount to add (can be negative to decrease).</param>
        public void IncrementProgress(float delta)
        {
            SetProgress(_progress + delta);
        }

        /// <summary>
        /// Reset progress to zero instantly.
        /// </summary>
        public void ResetProgress()
        {
            _progress = 0f;
            _displayedProgress = 0f;
            _wasComplete = false;
            ApplyVisuals(0f);
            OnFillReset.Invoke();
        }

        private void ApplyVisuals(float t)
        {
            if (_fillImage == null)
                return;

            _fillImage.fillAmount = t;

            if (_useGradientColor && _colorGradient != null)
                _fillImage.color = _colorGradient.Evaluate(t);
        }

        private void ApplyFillSettings()
        {
            if (_fillImage == null)
                return;

            _fillImage.type = Image.Type.Filled;
            _fillImage.fillMethod = _fillMethod;
            _fillImage.fillOrigin = (int)_fillOrigin360;
            _fillImage.fillClockwise = _clockwise;
        }

        private static Gradient CreateDefaultGradient()
        {
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(0.2f, 0.6f, 1f), 0f),
                    new GradientColorKey(new Color(0.2f, 1f, 0.4f), 0.5f),
                    new GradientColorKey(new Color(1f, 0.85f, 0.2f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            );
            return gradient;
        }
    }
}
