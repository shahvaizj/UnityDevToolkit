# Pause Manager

A global pause toggle that drives `Time.timeScale` with `OnPause`/`OnResume` UnityEvents and an optional pause key.

## Features

- Single persistent instance accessible from anywhere via `PauseManager.Instance`
- Pauses by zeroing `Time.timeScale` and restores the exact previous value on resume (slow-motion safe)
- `OnPause` and `OnResume` UnityEvents for wiring UI, audio, or gameplay systems in the Inspector
- Optional built-in pause key (defaults to Escape) — no extra input wiring needed
- Optional automatic `AudioListener` mute/unmute while paused
- `IsPaused` read-only property for querying state from other scripts

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/pausemanager.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add the **Pause Manager** component to a single persistent GameObject (e.g. a `GameManager` object in your first scene).
2. Leave **Dont Destroy On Load** on if the manager should survive scene loads.
3. Set **Enable Pause Key** and **Pause Key** if you want a built-in toggle key (defaults to Escape). Disable it if you'll drive pausing entirely from code or a UI button.
4. Wire up **On Pause** and **On Resume** in the Inspector to show/hide a pause menu, mute music, or freeze other systems.
5. From gameplay code, call `PauseManager.Instance.Pause()`, `Resume()`, or `TogglePause()` as needed.

### Scripting API

```csharp
using ShahvaizJ.PauseManager;

public class PauseMenuButton : MonoBehaviour
{
    public void OnResumeButtonPressed()
    {
        PauseManager.Instance.Resume();
    }

    public void OnPauseButtonPressed()
    {
        PauseManager.Instance.Pause();
    }
}
```

You can also read `PauseManager.Instance.IsPaused` to branch logic (e.g. ignore player input while paused).

## License

MIT — see [LICENSE](LICENSE).
