# Object Pooler

A keyed object pool manager with pre-warm, auto-grow, max-size limits, and IPoolable reset callbacks.

## Features

- Multiple named pools managed from a single component — one key per prefab type
- Pre-warm configurable instance counts at startup to avoid runtime allocation spikes
- Auto-grow creates new instances on demand when the pool is empty (optional, per pool)
- Max size cap prevents unbounded memory growth
- `IPoolable` interface with `OnSpawnFromPool` / `OnReturnToPool` callbacks for clean state reset
- `ReturnAll` to reclaim every active object in one call — great for scene resets
- Runtime pool registration and top-up via `RegisterPool` and `WarmUp`
- `OnObjectSpawned` and `OnObjectReturned` UnityEvents for audio, VFX, or analytics hooks
- Automatic hierarchy parenting keeps the scene tidy

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/objectpooler.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add the **Object Pooler** component to an empty GameObject (e.g. "PoolManager").
2. In the **Pools** list, click **+** to add a pool entry. Set a unique **Key** (e.g. "Bullet"), assign the **Prefab**, and choose a **Pre Warm Count**.
3. Optionally set **Max Size** to cap growth (0 = unlimited) and toggle **Auto Grow** on or off.
4. From gameplay code, call `Get("Bullet", position, rotation)` to spawn and `Return(instance)` to recycle.
5. Implement `IPoolable` on your pooled MonoBehaviours to receive reset callbacks.

### Scripting API

```csharp
using UnityEngine;
using ShahvaizJ.ObjectPooler;

public class Weapon : MonoBehaviour
{
    [SerializeField] private ObjectPooler _pool;
    [SerializeField] private Transform _muzzle;

    public void Fire()
    {
        GameObject bullet = _pool.Get("Bullet", _muzzle.position, _muzzle.rotation);
        if (bullet == null)
            Debug.Log("Pool exhausted!");
    }
}

// On the bullet prefab:
public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private ObjectPooler _pool;

    private float _timer;

    public void OnSpawnFromPool()
    {
        _timer = _lifetime;
    }

    public void OnReturnToPool()
    {
        // Clean up — stop particles, reset velocity, etc.
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            _pool.Return(gameObject);
    }
}
```

## License

MIT — see [LICENSE](LICENSE).
