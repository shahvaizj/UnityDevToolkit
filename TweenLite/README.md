# Tween Lite

A tiny code-driven tween helper for move/scale/rotate/fade/color with easing, loops, and delays — no external dependency required.

## Features

- One static entry point, `TweenLite`, plus `TweenMove` / `TweenScale` / `TweenRotate` / `TweenFade` / `TweenColor` extension methods
- 13 built-in easing curves (sine, quad, cubic, back) via the `Ease` enum
- Fluent configuration — chain `SetEase`, `SetDelay`, `SetLoops`, `SetUnscaledTime`, `OnComplete`, `OnKill` off the returned `Tween`
- Loop support with `Restart` or `PingPong` behaviour, including infinite loops
- No scene setup — a hidden runner GameObject is created automatically on first use
- Works with `Transform`, `CanvasGroup`, and `SpriteRenderer` out of the box, or drive any value with `TweenLite.To`

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/tweenlite.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. No component to add — `TweenLite` is a static API you call directly from any script.
2. Call an extension method like `transform.TweenMove(...)` or a static factory like `TweenLite.Scale(...)`.
3. Chain fluent setters on the returned `Tween` to configure easing, delay, or looping.
4. Store the returned `Tween` if you need to `Pause()`, `Play()`, or `Kill()` it later.
5. Call `TweenLite.KillAll()` on scene teardown if you have long-lived or infinite-looping tweens.

### Scripting API

```csharp
using UnityEngine;
using ShahvaizJ.TweenLite;

public class DoorAnimator : MonoBehaviour
{
    [SerializeField] private Transform _door;
    [SerializeField] private CanvasGroup _fadeGroup;

    private void OpenDoor()
    {
        _door.TweenMove(_door.position + Vector3.up * 2f, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => Debug.Log("Door open"));
    }

    private void PulseIcon(Transform icon)
    {
        icon.TweenScale(Vector3.one * 1.2f, 0.4f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.PingPong);
    }

    private void FadeOutUI()
    {
        _fadeGroup.TweenFade(0f, 0.3f).SetDelay(1f);
    }
}
```

## License

MIT — see [LICENSE](LICENSE).
