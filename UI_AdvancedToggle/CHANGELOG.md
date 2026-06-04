# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-05-25

### Added
- Initial release.
- `AdvancedToggle` MonoBehaviour with `NormalToggle`, `OnLimited`, `OffLimited`, and `OnOffLimited` modes.
- Generic visual feedback via `Graphic` (UI) and `Renderer` (3D) — both optional.
- `OnInteractionDisabled` UnityEvent replacing all project-specific interactable dependencies.
- Public scripting API: `Toggle()`, `SetState(bool)`, `ResetCounters()`, `SetLimits(int, int)`.
- `applyStateOnStart` flag to initialize visual/event state on `Start` without counting against limits.
- UPM-compatible `package.json` and assembly definition.
