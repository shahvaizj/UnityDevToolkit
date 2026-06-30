using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.CooldownButton
{
    /// <summary>
    /// Ability-style button that enters a timed cooldown after each click, showing a radial
    /// sweep overlay that drains as the cooldown expires, then fires <see cref="OnReady"/>.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Cooldown Button")]
    public class CooldownButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _cooldownOverlay;
        [SerializeField] private TMP_Text _cooldownLabel;

        [Header("Cooldown")]
        [SerializeField] private float _cooldownDuration = 3f;
        [SerializeField] private bool _disableButtonDuringCooldown = true;
        [SerializeField] private bool _useUnscaledTime = false;

        [Header("Overlay Fill")]
        [SerializeField] private Image.FillMethod _fillMethod = Image.FillMethod.Radial360;
        [SerializeField] private Image.Origin360 _fillOrigin = Image.Origin360.Top;
        [SerializeField] private bool _clockwise = true;

        [Header("Label")]
        [SerializeField] private bool _showRemainingTime = true;
        [SerializeField] private string _readyText = "";

        [Header("Events")]
        public UnityEvent OnClick;
        public UnityEvent OnCooldownStart;
        public UnityEvent OnReady;
        public UnityEvent<float> OnCooldownProgress;

        /// <summary>True when the button is not on cooldown and can be activated.</summary>
        public bool IsReady => !_isCoolingDown;

        /// <summary>Seconds remaining on the current cooldown, or 0 if ready.</summary>
        public float RemainingTime => _isCoolingDown
            ? _cooldownDuration - _elapsed
            : 0f;

        /// <summary>Elapsed fraction of the cooldown (0 = just started, 1 = finished).</summary>
        public float NormalizedProgress => _cooldownDuration > 0f
            ? Mathf.Clamp01(_elapsed / _cooldownDuration)
            : 1f;

        private bool _isCoolingDown;
        private float _elapsed;

        private void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            if (_button == null)
            {
                Debug.LogError("CooldownButton: no Button found on this GameObject. Assign one in the Inspector.", this);
                return;
            }

            _button.onClick.AddListener(HandleClick);
            InitOverlay();
            SetLabel(readyState: true);
        }

        private void Update()
        {
            if (!_isCoolingDown)
                return;

            _elapsed += _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _cooldownDuration);

            ApplyOverlay(1f - t);
            SetLabelFromProgress(t);
            OnCooldownProgress.Invoke(t);

            if (t >= 1f)
                CompleteCooldown();
        }

        /// <summary>
        /// Start the cooldown manually (same as a button click, but no OnClick event fires).
        /// Does nothing if the button is already cooling down.
        /// </summary>
        public void StartCooldown()
        {
            if (_isCoolingDown)
                return;

            _isCoolingDown = true;
            _elapsed = 0f;

            ApplyOverlay(1f);
            SetLabel(readyState: false);

            if (_disableButtonDuringCooldown && _button != null)
                _button.interactable = false;

            OnCooldownStart.Invoke();
        }

        /// <summary>
        /// Cancel and reset the cooldown, re-enabling the button immediately.
        /// </summary>
        public void ResetCooldown()
        {
            _isCoolingDown = false;
            _elapsed = 0f;

            ApplyOverlay(0f);
            SetLabel(readyState: true);

            if (_button != null)
                _button.interactable = true;
        }

        private void HandleClick()
        {
            OnClick.Invoke();
            StartCooldown();
        }

        private void CompleteCooldown()
        {
            _isCoolingDown = false;
            _elapsed = 0f;

            ApplyOverlay(0f);
            SetLabel(readyState: true);

            if (_disableButtonDuringCooldown && _button != null)
                _button.interactable = true;

            OnReady.Invoke();
        }

        private void InitOverlay()
        {
            if (_cooldownOverlay == null)
                return;

            _cooldownOverlay.type = Image.Type.Filled;
            _cooldownOverlay.fillMethod = _fillMethod;
            _cooldownOverlay.fillOrigin = (int)_fillOrigin;
            _cooldownOverlay.fillClockwise = _clockwise;
            _cooldownOverlay.fillAmount = 0f;
        }

        private void ApplyOverlay(float fillAmount)
        {
            if (_cooldownOverlay != null)
                _cooldownOverlay.fillAmount = fillAmount;
        }

        private void SetLabel(bool readyState)
        {
            if (_cooldownLabel == null || !_showRemainingTime)
                return;

            _cooldownLabel.text = readyState ? _readyText : _cooldownDuration.ToString("F1");
        }

        private void SetLabelFromProgress(float t)
        {
            if (_cooldownLabel == null || !_showRemainingTime)
                return;

            float remaining = _cooldownDuration * (1f - t);
            _cooldownLabel.text = remaining > 0.05f ? remaining.ToString("F1") : _readyText;
        }
    }
}
