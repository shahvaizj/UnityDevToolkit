# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-05

### Added
- Initial release.
- `TweenLite` static entry point with `To`, `Move`, `Scale`, `Rotate`, `Color`, and `KillAll`.
- `TweenExtensions` fluent helpers: `TweenMove`, `TweenScale`, `TweenRotate`, `TweenFade` (CanvasGroup/SpriteRenderer), `TweenColor`.
- `Tween` handle with `SetEase`, `SetDelay`, `SetLoops`, `SetUnscaledTime`, `OnComplete`, `OnKill`, `Pause`, `Play`, `Kill`.
- `Ease` enum with 13 curves (linear, sine, quad, cubic, back) and `LoopType.Restart` / `LoopType.PingPong`.
- Auto-created hidden runner GameObject — no manual scene setup required.
