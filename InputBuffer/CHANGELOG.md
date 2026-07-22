# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-23

### Added
- Initial release.
- InputBuffer component with per-action buffer windows and a shared default.
- WasPressed / TryConsume query API for polling buffered presses.
- OnBufferedInputConsumed UnityEvent for VFX/SFX hooks.
- Runtime Register / Unregister API for actions not configured in the Inspector.
- ClearAll to discard stale buffered input on state transitions.
