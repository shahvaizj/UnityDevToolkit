# Parallax Background

A multi-layer 2D parallax background system driven by camera movement, with optional seamless infinite scrolling per layer.

## Features

- `ParallaxLayer` component — attach one per background layer (sky, mountains, foreground) with its own independent parallax factor
- Per-axis parallax factor — control horizontal and vertical drift separately for each layer
- Optional seamless infinite horizontal/vertical scrolling for tiled sprites, so layers never run out
- Auto-detects `Camera.main` if no camera is assigned
- `SetCamera` and `ResetLayer` API for runtime camera swaps and level transitions without a visible snap

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/parallaxbackground.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add a **Parallax Layer** component to each background layer GameObject (e.g. `Sky`, `Mountains`, `Foreground`).
2. Leave **Camera Transform** empty to auto-use `Camera.main`, or assign a specific camera.
3. Set **Parallax Factor** per layer — closer layers (e.g. foreground) should use values near `1`, distant layers (e.g. sky) should use values near `0`.
4. For layers that need to repeat forever (e.g. a horizontally scrolling ground), enable **Infinite Horizontal** and/or **Infinite Vertical**, and use a sprite with the **Tiled** draw mode wide/tall enough to cover the camera view without a visible seam.
5. If you move the camera or a layer directly (e.g. on a level transition), call `ResetLayer()` on affected layers to avoid a visible jump.

### Scripting API

```csharp
using ShahvaizJ.ParallaxBackground;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private ParallaxLayer[] _layers;
    [SerializeField] private Transform _newCamera;

    public void OnLevelLoaded()
    {
        foreach (var layer in _layers)
            layer.SetCamera(_newCamera);
    }
}
```

## License

MIT — see [LICENSE](LICENSE).
