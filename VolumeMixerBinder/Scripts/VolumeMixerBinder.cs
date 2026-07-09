using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.VolumeMixerBinder
{
    /// <summary>
    /// Binds a UI <see cref="Slider"/> to a single exposed float parameter on an
    /// <see cref="AudioMixer"/>, converting the slider's linear 0-1 value to decibels and
    /// persisting the chosen value with <see cref="PlayerPrefs"/>. Add one instance per
    /// mixer group (master, music, SFX, etc.) and point each at its own exposed parameter name.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Volume Mixer Binder")]
    public class VolumeMixerBinder : MonoBehaviour
    {
        private const string PrefKeyPrefix = "VolumeMixerBinder.";
        private const float SilenceThreshold = 0.0001f;
        private const float DecibelMultiplier = 20f;

        [Header("Mixer Target")]
        [Tooltip("The AudioMixer that exposes the volume parameter to control.")]
        [SerializeField] private AudioMixer _audioMixer;

        [Tooltip("The name of the exposed float parameter on the AudioMixer (must be exposed via right-click > Expose in the Inspector).")]
        [SerializeField] private string _exposedParameter = "MasterVolume";

        [Header("UI")]
        [Tooltip("Slider bound to this parameter, using a linear 0-1 range.")]
        [SerializeField] private Slider _slider;

        [Header("Range")]
        [Tooltip("Decibel value applied when the slider is at 0 (effectively silent).")]
        [SerializeField] private float _minDecibels = -80f;

        [Tooltip("Linear volume (0-1) applied when no saved value exists yet.")]
        [SerializeField, Range(0f, 1f)] private float _defaultVolume = 1f;

        [Header("Behaviour")]
        [Tooltip("Load and apply the saved (or default) volume automatically on Awake.")]
        [SerializeField] private bool _loadSavedVolumeOnAwake = true;

        [Tooltip("Persist to PlayerPrefs automatically whenever the bound slider changes, instead of requiring an explicit ApplyAndSave() call.")]
        [SerializeField] private bool _saveOnSliderChange = true;

        [Header("Events")]
        [Tooltip("Fired with the new linear (0-1) volume whenever the mixer parameter is applied.")]
        public UnityEvent<float> OnVolumeChanged;

        /// <summary>The current linear (0-1) volume last applied to the mixer.</summary>
        public float CurrentVolume => _currentVolume;

        private float _currentVolume;
        private string PrefKey => PrefKeyPrefix + _exposedParameter;

        private void Awake()
        {
            if (_slider != null)
                _slider.onValueChanged.AddListener(HandleSliderChanged);

            if (_loadSavedVolumeOnAwake)
                LoadSavedVolume();
        }

        private void OnDestroy()
        {
            if (_slider != null)
                _slider.onValueChanged.RemoveListener(HandleSliderChanged);
        }

        private void HandleSliderChanged(float linearVolume)
        {
            SetVolume(linearVolume);

            if (_saveOnSliderChange)
                PlayerPrefs.SetFloat(PrefKey, linearVolume);
        }

        /// <summary>
        /// Applies a linear (0-1) volume to the bound <see cref="AudioMixer"/> parameter,
        /// converting it to decibels. Does not persist — call <see cref="ApplyAndSave"/>
        /// or enable <see cref="_saveOnSliderChange"/> to write the change to <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="linearVolume">Volume in the 0-1 range.</param>
        public void SetVolume(float linearVolume)
        {
            _currentVolume = Mathf.Clamp01(linearVolume);

            if (_audioMixer != null)
                _audioMixer.SetFloat(_exposedParameter, LinearToDecibel(_currentVolume, _minDecibels));

            OnVolumeChanged.Invoke(_currentVolume);
        }

        /// <summary>
        /// Writes the current slider value (or <see cref="CurrentVolume"/> if no slider is
        /// bound) to <see cref="PlayerPrefs"/> and flushes it to disk immediately.
        /// </summary>
        public void ApplyAndSave()
        {
            float volume = _slider != null ? _slider.value : _currentVolume;

            SetVolume(volume);
            PlayerPrefs.SetFloat(PrefKey, volume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Reads the saved linear volume from <see cref="PlayerPrefs"/> (falling back to
        /// <see cref="_defaultVolume"/> if none has been saved yet), updates the bound slider
        /// without re-triggering its change event, and applies the value to the mixer.
        /// </summary>
        public void LoadSavedVolume()
        {
            float volume = PlayerPrefs.GetFloat(PrefKey, _defaultVolume);

            if (_slider != null)
                _slider.SetValueWithoutNotify(volume);

            SetVolume(volume);
        }

        /// <summary>
        /// Resets the volume to <see cref="_defaultVolume"/>, updates the bound slider,
        /// applies the value to the mixer, and persists it to <see cref="PlayerPrefs"/>.
        /// </summary>
        public void ResetToDefault()
        {
            if (_slider != null)
                _slider.SetValueWithoutNotify(_defaultVolume);

            SetVolume(_defaultVolume);
            PlayerPrefs.SetFloat(PrefKey, _defaultVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Converts a linear volume (0-1) to a decibel value suitable for
        /// <see cref="AudioMixer.SetFloat(string, float)"/>, clamped to <paramref name="minDecibels"/>.
        /// </summary>
        /// <param name="linearVolume">Volume in the 0-1 range.</param>
        /// <param name="minDecibels">Decibel value to use at (or near) zero volume.</param>
        public static float LinearToDecibel(float linearVolume, float minDecibels)
        {
            if (linearVolume <= SilenceThreshold)
                return minDecibels;

            return Mathf.Max(Mathf.Log10(linearVolume) * DecibelMultiplier, minDecibels);
        }

        /// <summary>
        /// Converts a decibel value back to a linear volume (0-1). Inverse of <see cref="LinearToDecibel"/>.
        /// </summary>
        /// <param name="decibels">Decibel value, typically in the -80 to 0 range.</param>
        public static float DecibelToLinear(float decibels)
        {
            return Mathf.Pow(10f, decibels / DecibelMultiplier);
        }
    }
}
