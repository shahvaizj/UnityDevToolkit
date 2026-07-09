# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-09

### Added
- Initial release.
- `SettingsMenuKit` MonoBehaviour — binds volume, quality, resolution, and fullscreen UI controls to Unity's engine settings.
- Automatic population of quality and resolution dropdowns from `QualitySettings.names` and `Screen.resolutions`.
- `ApplyAndSave()`, `LoadSavedSettings()`, and `ResetToDefaults()` API backed by `PlayerPrefs`.
- `OnSettingsLoaded`, `OnSettingsApplied`, `OnSettingsReset` UnityEvents.
