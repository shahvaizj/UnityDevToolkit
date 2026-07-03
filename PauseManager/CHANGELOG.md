# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-03

### Added
- Initial release.
- PauseManager singleton component with Pause, Resume, and TogglePause methods.
- Automatic time-scale restore on resume, preserving slow-motion/speed-up values.
- OnPause and OnResume UnityEvents.
- Optional built-in pause key (default Escape).
- Optional AudioListener mute/unmute while paused.
- IsPaused read-only state property.
