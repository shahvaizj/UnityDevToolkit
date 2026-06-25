# Health System

A flexible health component with damage, healing, death events, and configurable invincibility frames.

## Features

- Damage and healing with clamped values — health never drops below zero or exceeds max
- Automatic invincibility frames (i-frames) after taking damage with configurable duration
- Kill, Revive, and ResetHealth methods for full lifecycle control
- Manual invincibility toggle with timed or permanent modes
- Six UnityEvents: `OnDamaged`, `OnHealed`, `OnHealthChanged`, `OnDeath`, `OnInvincibilityStarted`, `OnInvincibilityEnded`
- Scaled or unscaled time support for i-frames
- Read-only properties: `CurrentHealth`, `MaxHealth`, `HealthPercent`, `IsAlive`, `IsInvincible`

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/healthsystem.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add the **Health System** component to any GameObject — player, enemy, destructible prop, etc.
2. Set **Max Health** to the desired hit point total. Health starts at max when the component awakens.
3. Optionally set **I-Frame Duration** (in seconds) to grant automatic invincibility after each hit. Leave at 0 to disable.
4. Wire up **On Damaged**, **On Death**, and other UnityEvents in the Inspector for UI, audio, or VFX responses.
5. From gameplay code, call `TakeDamage()` when the entity is hit and `Heal()` to restore health.

### Scripting API

```csharp
using ShahvaizJ.HealthSystem;

public class Enemy : MonoBehaviour
{
    [SerializeField] private HealthSystem _health;

    private void Start()
    {
        _health.OnDeath.AddListener(HandleDeath);
    }

    public void OnHitByProjectile(float damage)
    {
        float dealt = _health.TakeDamage(damage);
        Debug.Log($"Dealt {dealt} damage, {_health.CurrentHealth}/{_health.MaxHealth} HP left");
    }

    public void OnPickupHealth(float amount)
    {
        _health.Heal(amount);
    }

    private void HandleDeath()
    {
        Debug.Log("Enemy destroyed!");
        Destroy(gameObject);
    }
}
```

You can also use `Kill()` for instant death, `Revive()` to bring a dead entity back, `SetInvincible()` for timed or permanent invincibility, and read `HealthPercent` (0–1) to drive health bars.

## License

MIT — see [LICENSE](LICENSE).
