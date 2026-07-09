# Volume Mixer Binder

Binds a UI slider to an AudioMixer exposed volume parameter, handling the linear-to-decibel conversion and persisting the chosen value with PlayerPrefs.

## Features

- Converts a linear 0-1 slider value to decibels for `AudioMixer.SetFloat`, with a configurable silence floor
- One component per exposed parameter — add one for Master, one for Music, one for SFX, etc.
- `ApplyAndSave()` / `LoadSavedVolume()` / `ResetToDefault()` API backed by `PlayerPrefs`
- Optional auto-save on every slider change, or explicit save via an Apply button
- `OnVolumeChanged` UnityEvent exposing the current linear volume for custom wiring (e.g. a percentage label)

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. In your `AudioMixer`, right-click the group you want to control and choose **Expose \<Parameter\> to script**, then rename it (e.g. `MasterVolume`) via the mixer's Exposed Parameters list.
2. Add a **Volume Mixer Binder** component to your options panel (one per parameter) and assign the **Audio Mixer** and matching **Exposed Parameter** name.
3. Assign the **Slider** it should control.
4. Set **Min Decibels** (typically `-80`) and **Default Volume** for first-run players.
5. Leave **Load Saved Volume On Awake** and **Save On Slider Change** checked for automatic persistence, or uncheck **Save On Slider Change** and wire an "Apply" button to `ApplyAndSave()` instead.

### Scripting API

```csharp
using ShahvaizJ.VolumeMixerBinder;

// Drive the mixer directly without touching the UI
volumeMixerBinder.SetVolume(0.75f);

// Persist whatever the slider currently shows
volumeMixerBinder.ApplyAndSave();

// Re-load and re-apply the saved value (e.g. after opening the menu)
volumeMixerBinder.LoadSavedVolume();

// Reset to the default volume and persist immediately
volumeMixerBinder.ResetToDefault();

// Static helpers for manual conversion elsewhere in your project
float db = VolumeMixerBinder.LinearToDecibel(0.5f, -80f);
float linear = VolumeMixerBinder.DecibelToLinear(db);
```

## License

MIT — see [LICENSE](LICENSE).
