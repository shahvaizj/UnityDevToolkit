using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.SettingsMenuKit
{
    /// <summary>
    /// Drives a standard options menu (master volume, quality level, resolution, fullscreen)
    /// and persists the chosen values to <see cref="PlayerPrefs"/>. Wire the Inspector fields to
    /// your UI controls and this component handles populating dropdowns from the current
    /// platform's available options, applying changes to the engine, and saving/loading between
    /// sessions.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Settings Menu Kit")]
    public class SettingsMenuKit : MonoBehaviour
    {
        private const string PrefKeyVolume = "SettingsMenuKit.Volume";
        private const string PrefKeyQualityLevel = "SettingsMenuKit.QualityLevel";
        private const string PrefKeyResolutionIndex = "SettingsMenuKit.ResolutionIndex";
        private const string PrefKeyFullscreen = "SettingsMenuKit.Fullscreen";

        [Header("Volume")]
        [Tooltip("Slider bound to the master volume, in the 0-1 range.")]
        [SerializeField] private Slider _volumeSlider;

        [Tooltip("Volume applied when no saved value exists yet.")]
        [SerializeField, Range(0f, 1f)] private float _defaultVolume = 1f;

        [Header("Quality")]
        [Tooltip("Dropdown populated from QualitySettings.names.")]
        [SerializeField] private TMP_Dropdown _qualityDropdown;

        [Header("Resolution")]
        [Tooltip("Dropdown populated from Screen.resolutions.")]
        [SerializeField] private TMP_Dropdown _resolutionDropdown;

        [Header("Fullscreen")]
        [Tooltip("Toggle bound to Screen.fullScreen.")]
        [SerializeField] private Toggle _fullscreenToggle;

        [Header("Behaviour")]
        [Tooltip("Load and apply saved settings (or defaults) automatically on Awake.")]
        [SerializeField] private bool _loadSavedSettingsOnAwake = true;

        [Header("Events")]
        [Tooltip("Fired after saved settings (or defaults) have been loaded and applied.")]
        public UnityEvent OnSettingsLoaded;

        [Tooltip("Fired after the current UI values have been applied to the engine and saved.")]
        public UnityEvent OnSettingsApplied;

        [Tooltip("Fired after settings have been reset to their defaults.")]
        public UnityEvent OnSettingsReset;

        private Resolution[] _availableResolutions;

        private void Awake()
        {
            PopulateQualityDropdown();
            PopulateResolutionDropdown();

            if (_volumeSlider != null)
                _volumeSlider.onValueChanged.AddListener(SetVolume);

            if (_qualityDropdown != null)
                _qualityDropdown.onValueChanged.AddListener(SetQualityLevel);

            if (_resolutionDropdown != null)
                _resolutionDropdown.onValueChanged.AddListener(SetResolution);

            if (_fullscreenToggle != null)
                _fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

            if (_loadSavedSettingsOnAwake)
                LoadSavedSettings();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Sets the master volume via <see cref="AudioListener.volume"/>. Does not persist —
        /// call <see cref="ApplyAndSave"/> to write the change to <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="volume">Volume in the 0-1 range.</param>
        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
        }

        /// <summary>
        /// Sets the active quality level via <see cref="QualitySettings.SetQualityLevel(int, bool)"/>.
        /// Does not persist — call <see cref="ApplyAndSave"/> to write the change to <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="qualityIndex">Index into <see cref="QualitySettings.names"/>.</param>
        public void SetQualityLevel(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
        }

        /// <summary>
        /// Applies a resolution from the populated <see cref="Screen.resolutions"/> list.
        /// Does not persist — call <see cref="ApplyAndSave"/> to write the change to <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="resolutionIndex">Index into the resolution list built on Awake.</param>
        public void SetResolution(int resolutionIndex)
        {
            if (_availableResolutions == null || resolutionIndex < 0 || resolutionIndex >= _availableResolutions.Length)
                return;

            Resolution resolution = _availableResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
        }

        /// <summary>
        /// Sets <see cref="Screen.fullScreen"/>. Does not persist — call <see cref="ApplyAndSave"/>
        /// to write the change to <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="isFullscreen">True to switch to fullscreen, false for windowed.</param>
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        /// <summary>
        /// Writes the current UI control values to <see cref="PlayerPrefs"/> and flushes them to
        /// disk. Call this from a "Confirm"/"Apply" button after the user has made changes.
        /// </summary>
        public void ApplyAndSave()
        {
            float volume = _volumeSlider != null ? _volumeSlider.value : _defaultVolume;
            int qualityIndex = _qualityDropdown != null ? _qualityDropdown.value : QualitySettings.GetQualityLevel();
            int resolutionIndex = _resolutionDropdown != null ? _resolutionDropdown.value : 0;
            bool isFullscreen = _fullscreenToggle != null ? _fullscreenToggle.isOn : Screen.fullScreen;

            PlayerPrefs.SetFloat(PrefKeyVolume, volume);
            PlayerPrefs.SetInt(PrefKeyQualityLevel, qualityIndex);
            PlayerPrefs.SetInt(PrefKeyResolutionIndex, resolutionIndex);
            PlayerPrefs.SetInt(PrefKeyFullscreen, isFullscreen ? 1 : 0);
            PlayerPrefs.Save();

            OnSettingsApplied.Invoke();
        }

        /// <summary>
        /// Reads saved values from <see cref="PlayerPrefs"/> (falling back to the current engine
        /// state or <see cref="_defaultVolume"/> for any key that has not been saved yet), updates
        /// the bound UI controls without re-triggering their change events, and applies the
        /// values to the engine.
        /// </summary>
        public void LoadSavedSettings()
        {
            float volume = PlayerPrefs.GetFloat(PrefKeyVolume, _defaultVolume);
            int qualityIndex = PlayerPrefs.GetInt(PrefKeyQualityLevel, QualitySettings.GetQualityLevel());
            int resolutionIndex = PlayerPrefs.GetInt(PrefKeyResolutionIndex, FindCurrentResolutionIndex());
            bool isFullscreen = PlayerPrefs.GetInt(PrefKeyFullscreen, Screen.fullScreen ? 1 : 0) == 1;

            ApplyValuesToUiAndEngine(volume, qualityIndex, resolutionIndex, isFullscreen);

            OnSettingsLoaded.Invoke();
        }

        /// <summary>
        /// Resets volume, quality, resolution, and fullscreen to their defaults, updates the UI,
        /// applies the values to the engine, and saves them to <see cref="PlayerPrefs"/>.
        /// </summary>
        public void ResetToDefaults()
        {
            int defaultQualityIndex = QualitySettings.names.Length - 1;
            int defaultResolutionIndex = FindCurrentResolutionIndex();

            ApplyValuesToUiAndEngine(_defaultVolume, defaultQualityIndex, defaultResolutionIndex, true);
            ApplyAndSave();

            OnSettingsReset.Invoke();
        }

        // ─── Internal ─────────────────────────────────────────────────────────

        private void ApplyValuesToUiAndEngine(float volume, int qualityIndex, int resolutionIndex, bool isFullscreen)
        {
            if (_volumeSlider != null)
                _volumeSlider.SetValueWithoutNotify(volume);
            SetVolume(volume);

            if (_qualityDropdown != null)
                _qualityDropdown.SetValueWithoutNotify(qualityIndex);
            SetQualityLevel(qualityIndex);

            if (_resolutionDropdown != null)
                _resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
            SetResolution(resolutionIndex);

            if (_fullscreenToggle != null)
                _fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
            SetFullscreen(isFullscreen);
        }

        private void PopulateQualityDropdown()
        {
            if (_qualityDropdown == null)
                return;

            _qualityDropdown.ClearOptions();
            _qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        }

        private void PopulateResolutionDropdown()
        {
            _availableResolutions = Screen.resolutions;

            if (_resolutionDropdown == null)
                return;

            var options = new System.Collections.Generic.List<string>(_availableResolutions.Length);
            foreach (Resolution resolution in _availableResolutions)
                options.Add($"{resolution.width} x {resolution.height} @ {resolution.refreshRateRatio.value:0}Hz");

            _resolutionDropdown.ClearOptions();
            _resolutionDropdown.AddOptions(options);
        }

        private int FindCurrentResolutionIndex()
        {
            if (_availableResolutions == null)
                return 0;

            for (int i = 0; i < _availableResolutions.Length; i++)
            {
                if (_availableResolutions[i].width == Screen.currentResolution.width &&
                    _availableResolutions[i].height == Screen.currentResolution.height)
                    return i;
            }

            return Mathf.Max(0, _availableResolutions.Length - 1);
        }
    }
}
