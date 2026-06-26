# Unity Developer Toolkit

A curated collection of reusable Unity tools, components, and utilities — built over years of real game development and open-sourced for the community.

---

## What Is This?

This repository is a growing library of production-ready Unity tools covering debugging, visualization, and common gameplay systems. Every tool here was born out of an actual need during development — nothing is included just to fill space.

Whether you're a solo indie developer racing against time or a studio looking for solid, drop-in utilities, this toolkit is built to save you hours.

---

## What's Included

### Gizmo Utilities
Drop-on components and a static helper library for scene debugging and visualization. Shapes are filled by default with a wireframe toggle in the Inspector.

- Components for every common shape: box, sphere, circle, cone, capsule, arrow, path
- Filled by default — toggle wireframe per component without touching code
- Static shape library: wire circle, arc, sector, cone, capsule, cylinder, torus
- Composite helpers: labeled spheres, range rings, grids, live raycasts, waypoint paths

### Custom Log
A lightweight conditional debug logger with colored console output. All calls are automatically stripped from non-development builds.

- Plain, warning, and error logs wrapping Unity's `Debug` class
- Colored variants: Blue, Red, Yellow, Green, Cyan, Orange
- All calls stripped in release builds via `[Conditional]`
- Optional context object to ping GameObjects from the console

### Timer Utility
A drop-in timer component with countdown/count-up modes, audio hooks, and event callbacks. No coroutines — driven by `Update`.

- Countdown and count-up modes with loop and auto-start support
- Audio hooks: one-shot (start/end/tick), looping background, last-warning trigger
- Decoupled `TimerUI` with TMP support, multiple time formats, and warning color
- Full `UnityEvent` callbacks: started, completed, stopped, paused, resumed

### Advanced Toggle
A flexible, zero-dependency Unity toggle MonoBehaviour with optional usage limits, generic visual feedback, and rich UnityEvent callbacks.

- Four toggle modes — unlimited, On-limited, Off-limited, or both limited independently
- Generic visual feedback — targets any `Renderer` (3D / SpriteRenderer) or `Graphic` (UI Image, Text, etc.)
- Rich events — `OnToggledOn`, `OnToggledOff`, `OnLimitReached`, `OffLimitReached`, `OnInteractionDisabled`
- No external dependencies — only `UnityEngine` and `UnityEngine.UI`

### HoldButton
A press-and-hold interaction component for Unity UI with configurable duration and callbacks.
- Configurable hold duration
- Visual progress feedback via target Graphic (UI) or Renderer (3D)
- `OnHoldComplete`, `OnHoldCancel`, `OnHoldProgress` UnityEvents
- Cancel on release before threshold

### CameraShake
A trauma-based camera shake component for Unity using Perlin noise, triggered with a single line of code.
- Trauma accumulation model — impacts add trauma, which decays smoothly over time
- Shake amplitude scales with trauma² (configurable exponent) so big hits punch and small ones stay subtle
- Separate translation and rotation amplitudes, set per axis
- Perlin-noise driven for smooth, organic motion (no harsh jitter)

### MultiSelect
A UI option selector with left/right arrow buttons that cycles through a list of string values.
- Left / right arrow buttons cycle through a serialized `string[]` of options
- Populates the label from the options on `Awake`
- `CurSelected` (index) and `Value` (text) exposed for reads
- Optional wrap-around — arrows auto-disable at the ends when wrapping is off

### ScreenFlash
A full-screen color flash overlay for hits, pickups, and transitions — driven by an AnimationCurve for complete control over the flash shape.
- One-line trigger via a static `Instance` — call `ScreenFlash.Instance.Flash()` from anywhere
- AnimationCurve-driven alpha envelope for sharp pops, slow fades, or any custom shape
- Configurable overlap modes: Restart, Ignore, or Enqueue when flashes overlap
- Scaled or unscaled time — keep flashing while the game is paused

### SmoothNumText
Smoothly animated number counter for score, coins, and currency displays using TextMeshPro.
- Smooth interpolation from current value to target — no coroutines, driven by `Update`
- AnimationCurve-driven easing for snappy ease-out, linear counting, or any custom shape
- Configurable prefix and suffix strings (e.g. "$", " coins", "Score: ")
- Whole-number or decimal display with optional thousands separators (1,000,000)

### SingletonBase
A generic MonoBehaviour singleton base class with automatic instance management, scene persistence, and duplicate handling.
- Generic `Singleton<T>` base class — one line to make any MonoBehaviour a singleton
- Optional `DontDestroyOnLoad` persistence across scene loads
- Configurable duplicate handling: destroy newest or destroy oldest
- Application-quit safety to prevent stale references during shutdown
- Virtual hooks (`OnInitialized`, `OnBeforeDestroyed`) for clean subclass lifecycle

### NotificationToast
Queue-based toast notification system with severity levels, slide animations, and per-severity styling.
- Four severity levels — Info, Success, Warning, Error — each with configurable background color, text color, and icon
- Smooth slide-in/slide-out animation from any direction (top, bottom, left, right) with cubic easing
- Automatic queue management — notifications display one at a time; extras queue up in order
- Configurable timing — independent fade-in, display, and fade-out durations
- Unscaled time support — toasts animate while the game is paused
- `OnToastShown` and `OnToastDismissed` UnityEvents for wiring gameplay responses

### Health System
A flexible health component with damage, healing, death events, and configurable invincibility frames.
- Damage and healing with clamped values — health never drops below zero or exceeds max
- Automatic invincibility frames (i-frames) after taking damage with configurable duration
- Kill, Revive, and ResetHealth methods for full lifecycle control
- Manual invincibility toggle with timed or permanent modes

### Object Pooler
A keyed object pool manager with pre-warm, auto-grow, max-size limits, and IPoolable reset callbacks.
- Multiple named pools managed from a single component — one key per prefab type
- Pre-warm configurable instance counts at startup to avoid runtime allocation spikes
- Auto-grow creates new instances on demand when the pool is empty (optional, per pool)
- Max size cap prevents unbounded memory growth
- `IPoolable` interface with `OnSpawnFromPool` / `OnReturnToPool` callbacks for clean state reset

---

## Getting Started

### Installation

**Option 1 — Unity Package Manager (Git URL)**

In Unity: `Window → Package Manager → + → Add package from git URL`

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

**Option 2 — Manual**

Clone or download this repository and drop the tool folder(s) you need into your project's `Assets/` directory.

```bash
git clone https://github.com/shahvaizj/UnityDevToolkit.git
```

### Requirements

- Unity **6000.0** or newer
- No third-party dependencies required

---

## Usage Examples

### Gizmo Utilities
```csharp
using ShahvaizJ.GizmoUtils;

private void OnDrawGizmosSelected()
{
    GizmoUtils.DrawLabeledSphere(transform.position, 5f, Color.yellow, "Detection");
    GizmoShapes.DrawWireSector(transform.position, Vector3.up, transform.forward, 60f, 5f, Color.cyan);
    GizmoArrow.Draw(transform.position, transform.forward, 2f, Color.blue);
}
```

### Custom Log
```csharp
using ShahvaizJ.CustomLog;

Log.Print("hello");
Log.Warning("watch out");
Log.Error("something broke");

Log.Blue("spawned");
Log.Green("health full");
Log.Yellow("low ammo");
Log.Red("took damage");
```

### Timer Utility
```csharp
using ShahvaizJ.TimerUtil;

// Wire TimerController and TimerUI in the Inspector, then control via code:
TimerController timer = GetComponent<TimerController>();
timer.onTimerCompleted.AddListener(() => Debug.Log("Time's up!"));
timer.StartTimer();

// Read state any time
float progress = timer.NormalizedProgress; // 0–1, useful for driving UI fills
```

---

## License

This project is licensed under the **MIT License** — free to use in personal and commercial projects. See [LICENSE](LICENSE) for full terms.

---

## Author

Made by **Shahvaiz** — indie game developer, currently building [*You May Never Leave*](https://shahvaizj.itch.io/you-may-never-leave), a paranormal sci-fi horror puzzle game.

If this toolkit saves you time, a star on the repo goes a long way.

---

*Built from real projects. Shared for everyone.*
