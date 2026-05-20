# Timer Utility

A drop-in timer component for Unity with audio, UI, and event support. No coroutines — driven by `Update`.

- Countdown and count-up modes with loop and auto-start support
- Interval tick sounds, looping background audio, and a last-warning audio trigger
- Decoupled `TimerUI` component with multiple time formats and warning color
- Full `UnityEvent` hooks: started, completed, stopped, paused, resumed
- Runtime Inspector controls for testing without writing code

## Requirements

- Unity 6000.0 or newer
- TextMeshPro (included with Unity 6)

## Installation

Drop the `TimerUtil/` folder into your project's `Assets/` directory, or add via UPM git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

## Setup

1. Add `TimerController` to a GameObject in your scene
2. Add `TimerUI` to a UI GameObject and assign a `TMP_Text` reference
3. Link the `TimerUI` on the `TimerController` Inspector
4. Wire audio clips and UnityEvents as needed
5. Enable **Auto Start**, or call `StartTimer()` from code

## TimerController

| Field | Description |
|---|---|
| Duration | Total timer length in seconds |
| Count Down | Show time remaining (on) or elapsed (off) |
| Auto Start | Starts automatically on scene load |
| Loop | Restarts automatically on completion |

**Audio**

| Field | Description |
|---|---|
| Start Sound | One-shot on timer start |
| End Sound | One-shot on timer completion |
| Tick Sound | One-shot at every N% of progress |
| Tick Percentage Interval | How often (%) the tick sound plays |
| Loop BG Source / Sound | Looping audio for the full duration |
| Last Warning Source / Sound | Looping audio triggered N seconds before end |
| Last Warning Seconds | How early the warning audio starts |

**Events**

`onTimerStarted` · `onTimerCompleted` · `onTimerStopped` · `onTimerPaused` · `onTimerResumed`

**Code API**

```csharp
using ShahvaizJ.TimerUtil;

TimerController timer = GetComponent<TimerController>();

timer.StartTimer();
timer.PauseTimer();
timer.ResumeTimer();
timer.StopTimer();
timer.ResetTimer();
timer.SetDuration(30f);

float elapsed  = timer.CurrentTime;
float progress = timer.NormalizedProgress; // 0–1
bool  running  = timer.IsRunning;
bool  paused   = timer.IsPaused;
```

## TimerUI

`TimerUI` is fully decoupled — it only needs to be fed data via `Tick()` and has no dependency on `TimerController`. You can drive it from any source.

| Field | Description |
|---|---|
| UI Root | Panel to show/hide (defaults to this GameObject) |
| Time Text | TMP_Text that displays the formatted time |
| Format | `mm:ss` · `mm:ss.f` · `s` · `s.f` |
| Use Warning Color | Tints the text when time crosses the threshold |
| Warning Threshold | Seconds remaining (or elapsed) to trigger the color |
| Normal Color / Warning Color | Colors used for the two states |

## License

MIT — free to use in personal and commercial projects.
