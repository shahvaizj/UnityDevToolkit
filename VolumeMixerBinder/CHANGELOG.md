# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-09

### Added
- Initial release.
- `VolumeMixerBinder` MonoBehaviour — binds a UI slider to an AudioMixer exposed float parameter.
- Linear-to-decibel conversion with a configurable silence floor, plus static `LinearToDecibel`/`DecibelToLinear` helpers.
- `ApplyAndSave()`, `LoadSavedVolume()`, and `ResetToDefault()` API backed by `PlayerPrefs`.
- Optional auto-save on slider change, or explicit save via an Apply button.
- `OnVolumeChanged` UnityEvent exposing the current linear volume.
