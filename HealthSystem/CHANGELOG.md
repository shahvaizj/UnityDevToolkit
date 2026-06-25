# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-06-25

### Added
- Initial release.
- HealthSystem component with configurable max health and reset-on-enable.
- TakeDamage and Heal methods with clamped return values.
- Automatic invincibility frames (i-frames) after damage with configurable duration.
- Manual invincibility via SetInvincible and ClearInvincibility.
- Kill, Revive, and ResetHealth lifecycle methods.
- SetMaxHealth for runtime max-health changes.
- OnDamaged, OnHealed, OnHealthChanged, OnDeath, OnInvincibilityStarted, OnInvincibilityEnded UnityEvents.
- Scaled and unscaled time support for i-frame countdown.
