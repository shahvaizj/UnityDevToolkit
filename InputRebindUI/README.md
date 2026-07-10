# Input Rebind UI

Runtime control rebinding UI for Unity's Input System, with automatic save and load of player-chosen bindings.

## Features

- Interactive rebinding for any action/binding on Unity's new Input System
- Works with simple bindings and composite parts (e.g. WASD up/down/left/right)
- Excludes noisy controls (mouse movement) and supports a configurable cancel control
- Per-row `OnRebindStarted` / `OnRebindComplete` / `OnRebindCanceled` UnityEvents for "press any key" prompts
- `InputRebindManager` persists all overrides to `PlayerPrefs` as JSON, loads them on startup, and can reset everything back to defaults

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/inputrebindui.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add an **Input Rebind Manager** component to a persistent GameObject (e.g. a bootstrap or menu scene) and assign your project's **Input Action Asset**.
2. For each rebindable control, add a **Rebind Action UI** component to its UI row and assign an **Action Reference** and **Binding Index** (0 for a simple binding, or the composite part index for composites like WASD).
3. Assign the row's **Rebind Button**, **Binding Display Text**, and optionally an **Action Name Text** and a "waiting for input" overlay GameObject.
4. Optionally wire a "Reset" button to the row's `ResetBinding()`, or a global "Reset All" button to `InputRebindManager.ResetAllBindings()`.
5. Bindings save automatically after each successful rebind and are reloaded automatically on `Awake` — no extra wiring needed.

### Scripting API

```csharp
using ShahvaizJ.InputRebindUI;

RebindActionUI rebindRow = GetComponent<RebindActionUI>();
rebindRow.StartRebind();   // begin an interactive rebind for this row
rebindRow.ResetBinding();  // revert this row's binding to its default

InputRebindManager.Instance.SaveBindings();
InputRebindManager.Instance.LoadBindings();
InputRebindManager.Instance.ResetAllBindings();
```

## License

MIT — see [LICENSE](LICENSE).
