# HoldButton

A press-and-hold interaction component for Unity UI with configurable duration and callbacks.

## Features

- Configurable hold duration
- Visual progress feedback via target Graphic (UI) or Renderer (3D)
- `OnHoldComplete`, `OnHoldCancel`, `OnHoldProgress` UnityEvents
- Cancel on release before threshold
- Progress value exposed as 0–1 float for custom wiring

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/holdbutton.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add a **HoldButton** component to any UI Button or GameObject with a graphic child.
2. Set **Hold Duration** (seconds) in the Inspector.
3. Optionally assign a **Target Graphic** or **Target Renderer** for automatic fill/color feedback.
4. Wire the **Events** in the Inspector, or subscribe from code.

### Scripting API

```csharp
using ShahvaizJ.HoldButton;

HoldButton hold = GetComponent<HoldButton>();

hold.Progress   // read current progress 0–1 (read-only)
hold.IsHolding  // is the pointer currently held down (read-only)

hold.Pause();
hold.Resume();
hold.Cancel();  // cancel the current hold and fire OnHoldCancel
```

## License

MIT — see [LICENSE](LICENSE).
