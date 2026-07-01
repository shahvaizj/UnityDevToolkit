# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-01

### Added
- Initial release.
- `ScreenFader` MonoBehaviour with `FadeOut`, `FadeIn`, and `FadeOutHoldIn` methods.
- Optional `Action` callback on all fade methods for coroutine and async pipeline integration.
- AnimationCurve-driven easing with Inspector-configurable curve.
- `SnapOpaque` and `SnapTransparent` for instant no-animation transitions.
- `SetFadeColor` to change the overlay color at runtime.
- Unscaled time mode — fades animate while `Time.timeScale = 0`.
- `OnFadeOutComplete` and `OnFadeInComplete` UnityEvents.
- `IsFading`, `IsOpaque`, `IsTransparent`, and `Alpha` state properties.
- Static `Instance` reference for global fire-and-forget access.
