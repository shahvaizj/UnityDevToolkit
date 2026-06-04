# Advanced Toggle

A flexible, zero-dependency Unity toggle MonoBehaviour with optional usage limits, generic visual feedback, and rich UnityEvent callbacks.

## Features

- **Four toggle modes** — unlimited, On-limited, Off-limited, or both limited independently  
- **Generic visual feedback** — targets any `Renderer` (3D / SpriteRenderer) or `Graphic` (UI Image, Text, etc.) — or neither  
- **Rich events** — `OnToggledOn`, `OnToggledOff`, `OnLimitReached`, `OffLimitReached`, `OnInteractionDisabled`  
- **No external dependencies** — only `UnityEngine` and `UnityEngine.UI`

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/advanced-toggle.git
```

Or copy the `Runtime/` folder into your project's `Assets/`.

## Usage

1. Add the **Advanced Toggle** component to any GameObject.
2. Set **Toggle Mode** and limits in the Inspector.
3. Wire callbacks in the **Events** section, or call `Toggle()` / `SetState(bool)` from code.
4. Optionally assign a **Target Graphic** (UI) or **Target Renderer** (3D) and choose On/Off colors for automatic color feedback.

### Scripting API

```csharp
AdvancedToggle toggle = GetComponent<AdvancedToggle>();

toggle.Toggle();               // flip state
toggle.SetState(true);         // force On
toggle.SetState(false);        // force Off

bool current = toggle.IsOn;    // read state

toggle.ResetCounters();        // re-enable exhausted limited modes
toggle.SetLimits(3, 3);        // change limits at runtime
```

### Toggle Modes

| Mode | Behaviour |
|---|---|
| `NormalToggle` | Unlimited flips in both directions |
| `OnLimited` | On direction fires at most `maxOnCount` times |
| `OffLimited` | Off direction fires at most `maxOffCount` times |
| `OnOffLimited` | Both directions independently limited; `OnInteractionDisabled` fires when both are exhausted |

## License

MIT — see [LICENSE](LICENSE).
