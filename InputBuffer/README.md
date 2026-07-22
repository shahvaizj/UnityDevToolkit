# Input Buffer

Fighting-game style input buffering for Unity's Input System — queries whether an action was pressed within the last N seconds for responsive combos.

## Features

- Buffers presses for any `InputAction` so a query made a few frames later still sees them
- Per-action buffer window overrides, with a shared default (seconds, not fixed frame counts)
- `WasPressed` (peek) and `TryConsume` (peek + clear) query styles for polling gameplay code
- `OnBufferedInputConsumed` UnityEvent for hooking VFX/SFX to a buffered input firing
- Runtime `Register`/`Unregister` API for actions not wired up in the Inspector
- `ClearAll` to discard stale buffered input on state transitions (e.g. entering a cutscene)

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/inputbuffer.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add an **Input Buffer** component to a persistent GameObject (e.g. your player controller).
2. Set the **Default Buffer Window** in seconds (0.15s ≈ 9 frames at 60fps is a good starting point).
3. Add entries to **Buffered Actions**, assigning an **Action Reference** for each action you want buffered, with an optional per-action **Buffer Window Override**.
4. Leave **Auto Enable Actions** on so the buffer enables/disables its actions itself, or turn it off if a `PlayerInput` component already manages them.
5. From your gameplay code (e.g. an attack state), call `TryConsume` each frame to check for a buffered press and act on it exactly once.

### Scripting API

```csharp
using ShahvaizJ.InputBuffer;
using UnityEngine.InputSystem;

InputBuffer buffer = GetComponent<InputBuffer>();

// In your combo/attack logic, run every frame:
if (buffer.TryConsume(_attackActionReference))
{
    TriggerComboAttack();
}

// Or just peek without consuming:
bool jumpBuffered = buffer.WasPressed(_jumpActionReference);

// Register an action at runtime instead of via the Inspector:
buffer.Register(_dodgeActionReference, bufferWindowOverride: 0.2f);

buffer.ClearAll(); // discard stale buffered input, e.g. before a cutscene
```

## License

MIT — see [LICENSE](LICENSE).
