# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-06

### Added
- Initial release.
- `ConfirmationDialog` MonoBehaviour — reusable modal yes/no confirmation popup.
- `Show(...)` API with `Action<bool>` result callback and per-call title/message/button-label overrides.
- `OnOpened`, `OnClosed`, `OnConfirmed`, `OnCancelled` UnityEvents.
- Static `Instance` for global access.
- `Close()` for dismissing programmatically without a confirm/cancel result.
