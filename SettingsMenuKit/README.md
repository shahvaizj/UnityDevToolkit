# Settings Menu Kit

A drop-in options menu component that binds volume, quality, resolution, and fullscreen controls to Unity's engine settings and persists them with PlayerPrefs.

## Features

- Populates quality and resolution dropdowns automatically from `QualitySettings.names` and `Screen.resolutions`
- Binds a volume slider, quality dropdown, resolution dropdown, and fullscreen toggle in one component
- `ApplyAndSave()` / `LoadSavedSettings()` / `ResetToDefaults()` API for Confirm/Cancel/Reset buttons
- Loads and applies saved settings automatically on `Awake` (optional)
- `OnSettingsLoaded`, `OnSettingsApplied`, `OnSettingsReset` UnityEvents for Inspector-driven wiring

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Build an options panel with a volume `Slider`, a quality `TMP_Dropdown`, a resolution `TMP_Dropdown`, and a fullscreen `Toggle`. Attach the **Settings Menu Kit** component to the panel's root GameObject.
2. Assign **Volume Slider**, **Quality Dropdown**, **Resolution Dropdown**, and **Fullscreen Toggle** in the Inspector.
3. Set **Default Volume** for first-run players who have no saved settings yet.
4. Leave **Load Saved Settings On Awake** checked so returning players see their saved options applied automatically.
5. Wire an "Apply" button's `onClick` to `ApplyAndSave()`, and optionally a "Reset" button to `ResetToDefaults()`.

### Scripting API

```csharp
using ShahvaizJ.SettingsMenuKit;

// Save whatever the player currently has set in the UI
settingsMenuKit.ApplyAndSave();

// Re-load and re-apply saved settings (e.g. after opening the menu)
settingsMenuKit.LoadSavedSettings();

// Reset everything to defaults and persist immediately
settingsMenuKit.ResetToDefaults();

// Drive a single setting directly without touching the UI
settingsMenuKit.SetVolume(0.75f);
```

## License

MIT — see [LICENSE](LICENSE).
