# Damage Number Popup

Pooled floating world-space damage/heal numbers with critical-hit styling and arc motion.

## Features

- Pooled spawning via `DamageNumberSpawner` — pre-warms instances on `Awake`, recycles them on completion, no per-hit allocation
- Upward arc motion with configurable initial speed, sideways spread, and gravity
- Automatic fade-out over the back half of each popup's lifetime
- Distinct color, scale multiplier, and suffix for critical hits (e.g. bigger, orange, `"!"`)
- Optional camera billboarding so numbers always face the player
- Custom text override for non-numeric popups (e.g. `"MISS"`, `"BLOCKED"`)

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/damagenumberpopup.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Create a prefab with a `TextMeshPro` (3D) text component and add the `DamageNumberPopup` component to the same GameObject — assign the label if it isn't auto-detected.
2. Tune the popup's Arc Motion, Fade, and Crit Styling fields in the Inspector to taste.
3. Add a `DamageNumberSpawner` component to a manager GameObject in your scene, assign the prefab to its Popup Prefab field, and set a Prewarm Count.
4. Optionally assign your main camera to the spawner's Facing Camera field so popups billboard toward the player.
5. Call `Spawn(worldPosition, amount, isCrit)` from your damage/heal code whenever you want a number to appear.

### Scripting API

```csharp
using UnityEngine;
using ShahvaizJ.DamageNumberPopup;

public class Enemy : MonoBehaviour
{
    [SerializeField] private DamageNumberSpawner _damageNumbers;

    public void TakeDamage(float amount, bool isCrit)
    {
        _damageNumbers.Spawn(transform.position, amount, isCrit);
    }

    public void Miss()
    {
        _damageNumbers.Spawn(transform.position, 0f, false, "MISS");
    }
}
```

## License

MIT — see [LICENSE](LICENSE).
