# Scriptable Variables

ScriptableObject-based Float, Int, Bool, and String variables with change events for decoupling systems without singletons.

## Features

- `FloatVariable`, `IntVariable`, `BoolVariable`, and `StringVariable` assets — create them from the `Assets/Create/ShahvaizJ/Scriptable Variables` menu
- Any script or component reads/writes the same shared value by referencing the asset directly — no singleton, no manual event wiring
- `OnValueChanged` UnityEvent per type, wireable in the Inspector, only fires when the value actually changes
- Runtime value automatically resets to the serialized Initial Value on Play Mode start (toggle via **Reset On Play**), so edits from a previous play session never leak into the next
- `ResetValue()` to manually restore the initial value, plus type-specific helpers (`Add` for Float/Int, `Toggle` for Bool)

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/scriptablevariables.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Right-click in the Project window and choose **Create > ShahvaizJ > Scriptable Variables > \[Float/Int/Bool/String] Variable**.
2. Set the **Initial Value** in the Inspector — this is the value the asset resets to at the start of each Play session.
3. Drag the asset into any `MonoBehaviour` field that needs it (declare the field as `FloatVariable`, `IntVariable`, `BoolVariable`, or `StringVariable`).
4. Read or write `variable.Value` from any script that holds a reference — every reader sees the same live value.
5. Wire the asset's **On Value Changed** UnityEvent in the Inspector (or subscribe in code) to react to changes, e.g. updating a health bar or score label.

### Scripting API

```csharp
using ShahvaizJ.ScriptableVariables;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private FloatVariable _health;

    private void Start()
    {
        _health.Value = _health.InitialValue;
    }

    public void TakeDamage(float amount)
    {
        _health.Add(-amount);
    }
}

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private FloatVariable _health;

    private void OnEnable() => _health.OnValueChanged.AddListener(HandleHealthChanged);
    private void OnDisable() => _health.OnValueChanged.RemoveListener(HandleHealthChanged);

    private void HandleHealthChanged(float newValue)
    {
        // update a slider, text, etc.
    }
}
```

`PlayerHealth` and `HealthBarUI` never reference each other — both only reference the shared `FloatVariable` asset.

## License

MIT — see [LICENSE](LICENSE).
