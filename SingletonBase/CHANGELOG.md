# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-06-22

### Added
- Initial release.
- Generic `Singleton<T>` MonoBehaviour base class.
- Configurable `DontDestroyOnLoad` scene persistence.
- `DuplicatePolicy` enum with DestroyNewest and DestroyOldest options.
- Application-quit guard to prevent stale instance access during shutdown.
- `OnInitialized` and `OnBeforeDestroyed` virtual lifecycle hooks.
