# Cooldown Button

An ability-style UI button with a radial cooldown sweep overlay and an `OnReady` event when the cooldown expires.

## Features

- Radial fill overlay drains over the configured cooldown duration after each button click
- Optional TMP label displaying remaining seconds during cooldown
- `OnClick`, `OnCooldownStart`, `OnReady`, and `OnCooldownProgress` UnityEvents
- Auto-disable button during cooldown (optional, toggleable)
- Scaled or unscaled time — cooldown ticks while the game is paused when unscaled
- `StartCooldown()` and `ResetCooldown()` API for script-driven control

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add a **Button** to your Canvas and attach the **Cooldown Button** component to the same GameObject.
2. Assign the **Button** reference in the Inspector (auto-detected from the same GameObject if left empty).
3. Create a child **Image** with `Image Type = Filled`, `Fill Method = Radial 360`, and assign it to **Cooldown Overlay**. Give it a semi-transparent dark color to act as the sweep mask.
4. Optionally assign a **TMP Text** child to **Cooldown Label** for the remaining-time counter.
5. Set **Cooldown Duration** (seconds) and configure the **Events** in the Inspector.
6. Press Play — clicking the button triggers the cooldown sweep automatically.

### Scripting API

```csharp
using ShahvaizJ.CooldownButton;

CooldownButton cooldown = GetComponent<CooldownButton>();

// Trigger cooldown programmatically (e.g. from an ability system)
cooldown.StartCooldown();

// Cancel an active cooldown and re-enable the button immediately
cooldown.ResetCooldown();

// Query state
if (cooldown.IsReady)
    Debug.Log("Button is ready!");

float seconds = cooldown.RemainingTime;   // seconds left
float progress = cooldown.NormalizedProgress; // 0–1 (1 = finished)

// Subscribe to events
cooldown.OnReady.AddListener(() => Debug.Log("Cooldown complete!"));
cooldown.OnCooldownProgress.AddListener(t => Debug.Log($"Progress: {t:P0}"));
```

## License

MIT — see [LICENSE](LICENSE).
