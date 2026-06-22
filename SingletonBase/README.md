# SingletonBase

A generic MonoBehaviour singleton base class for Unity with automatic instance management, scene persistence, and duplicate handling.

## Features

- Generic `Singleton<T>` base class — one line to make any MonoBehaviour a singleton
- Optional `DontDestroyOnLoad` persistence across scene loads
- Configurable duplicate handling: destroy newest or destroy oldest
- Application-quit safety to prevent stale references during shutdown
- Virtual hooks (`OnInitialized`, `OnBeforeDestroyed`) for clean subclass lifecycle

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Create a new C# script for your manager (e.g. `GameManager.cs`).
2. Inherit from `Singleton<GameManager>` instead of `MonoBehaviour`.
3. Override `OnInitialized()` for setup logic (replaces `Awake`).
4. In the Inspector, configure **Persist Across Scenes** and **Duplicate Policy**.
5. Access the instance from anywhere via `GameManager.Instance`.

### Scripting API

```csharp
using ShahvaizJ.SingletonBase;

public class GameManager : Singleton<GameManager>
{
    protected override void OnInitialized()
    {
        // Called once when this becomes the active singleton.
    }

    protected override void OnBeforeDestroyed()
    {
        // Called just before the singleton is destroyed.
    }
}

// Access from anywhere:
GameManager.Instance.DoSomething();

// Safe null check:
if (GameManager.HasInstance)
    GameManager.Instance.DoSomething();
```

## License

MIT — see [LICENSE](LICENSE).
