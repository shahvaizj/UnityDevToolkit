# Camera Follow 2D

A smooth-damped 2D camera follow component with a configurable deadzone and optional world-bounds clamping.

## Features

- `Vector3.SmoothDamp`-driven follow with configurable smooth time and max speed — no jitter, no snapping
- Deadzone rectangle — the target can move freely near the camera center before it starts following
- Optional world-bounds clamping that accounts for the camera's orthographic viewport size so edges never show past the level
- `SetTarget`, `SetBounds`, and `SnapToTarget` API for runtime target swaps, teleports, and respawns
- Unscaled time support — keeps tracking while `Time.timeScale` is 0
- Deadzone and bounds gizmos drawn in the Scene view when selected

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/camerafollow2d.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add the **Camera Follow 2D** component to your `Camera` GameObject (a `Camera` component is required).
2. Assign **Target** to the Transform the camera should follow (e.g. the player).
3. Tune **Deadzone Size** so small target movements near the center don't move the camera.
4. Tune **Smooth Time** and **Max Speed** to taste — lower smooth time follows more tightly.
5. Optionally enable **Use Bounds** and set **Min Bounds** / **Max Bounds** to clamp the camera to your level extents.

### Scripting API

```csharp
using ShahvaizJ.CameraFollow2D;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private CameraFollow2D _followCam;

    public void OnPlayerRespawn(Transform player)
    {
        _followCam.SetTarget(player);
        _followCam.SnapToTarget();
    }

    public void OnLevelLoaded(Vector2 min, Vector2 max)
    {
        _followCam.SetBounds(min, max);
    }
}
```

## License

MIT — see [LICENSE](LICENSE).
