# SmoothNumText

Smoothly animated number counter for score, coins, and currency displays using TextMeshPro.

## Features

- Smooth interpolation from current value to target — no coroutines, driven by `Update`
- AnimationCurve-driven easing for snappy ease-out, linear counting, or any custom shape
- Configurable prefix and suffix strings (e.g. "$", " coins", "Score: ")
- Whole-number or decimal display with optional thousands separators (1,000,000)
- Additive `AddValue` API for incremental changes (coin pickups, combo counters)
- `SnapToTarget` to cancel mid-animation and jump to the final value instantly
- Scaled or unscaled time — keep counting while the game is paused
- `OnAnimationStarted`, `OnAnimationComplete`, and `OnValueChanged` UnityEvents

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/smoothnumtext.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add a **TextMeshPro** text element to your Canvas (or world-space TMP).
2. Add the **Smooth Num Text** component to the same GameObject (or any GameObject).
3. Drag the TMP label into the **Label** field in the Inspector.
4. Set **Prefix** and **Suffix** to frame the number (e.g. prefix `"$"`, suffix `" coins"`).
5. Choose **Decimal Places** (0 for whole numbers) and toggle **Use Thousands Separator**.
6. Adjust **Default Duration** and the **Easing Curve** to taste.
7. From gameplay code, call `AnimateTo`, `AddValue`, or `SetValue` whenever the number changes.

### Scripting API

```csharp
using ShahvaizJ.SmoothNumText;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private SmoothNumText _scoreDisplay;

    private int _score;

    public void AddScore(int points)
    {
        _score += points;
        _scoreDisplay.AnimateTo(_score);
    }

    public void ResetScore()
    {
        _score = 0;
        _scoreDisplay.SetValue(0);
    }
}
```

You can also use the static `Instance` for quick access:

```csharp
SmoothNumText.Instance.AddValue(100);
```

## License

MIT — see [LICENSE](LICENSE).
