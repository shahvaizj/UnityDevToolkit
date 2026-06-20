# ScreenFlash

A full-screen color flash overlay for hits, pickups, and transitions — driven by an AnimationCurve for complete control over the flash shape.

## Features

- One-line trigger via a static `Instance` — call `ScreenFlash.Instance.Flash()` from anywhere
- AnimationCurve-driven alpha envelope for sharp pops, slow fades, or any custom shape
- Configurable overlap modes: Restart, Ignore, or Enqueue when flashes overlap
- Scaled or unscaled time — keep flashing while the game is paused
- `OnFlashStarted` / `OnFlashComplete` UnityEvents for audio or VFX hooks
- Pairs naturally with CameraShake for maximum game juice

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/screenflash.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Create a **Canvas** (Screen Space – Overlay) and add a **UI Image** that stretches across the entire screen. Set its color alpha to 0.
2. Add the **Screen Flash** component to any GameObject.
3. Drag the full-screen Image into the **Overlay** slot.
4. Tweak **Default Color**, **Default Duration**, and **Default Curve** in the Inspector to taste.
5. From gameplay code, call `Flash()` whenever something impactful happens.
6. Optionally wire **On Flash Started** / **On Flash Complete** in the Inspector for sound effects or post-processing.

### Scripting API

```csharp
using ShahvaizJ.ScreenFlash;

public class Player : MonoBehaviour
{
    private void TakeDamage()
    {
        // Quick red flash with Inspector defaults.
        ScreenFlash.Instance.Flash(Color.red);
    }

    private void PickupCoin()
    {
        // Short gold flash, custom duration.
        ScreenFlash.Instance.Flash(new Color(1f, 0.85f, 0f), 0.15f);
    }

    private void Die()
    {
        // Slow fade to black with a custom curve.
        var curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        ScreenFlash.Instance.Flash(Color.black, 1.5f, curve);
    }
}
```

### Overlap Modes

| Mode | Behaviour |
|---|---|
| **Restart** | Cancel the current flash and start the new one immediately |
| **Ignore** | Discard the new flash, let the current one finish |
| **Enqueue** | Queue the new flash to play after the current one ends |

## License

MIT — see [LICENSE](LICENSE).
