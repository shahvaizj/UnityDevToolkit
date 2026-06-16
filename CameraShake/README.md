# CameraShake

A trauma-based camera shake component for Unity using Perlin noise, triggered with a single line of code.

## Features

- Trauma accumulation model — impacts add trauma, which decays smoothly over time
- Shake amplitude scales with trauma² (configurable exponent) so big hits punch and small ones stay subtle
- Separate translation and rotation amplitudes, set per axis
- Perlin-noise driven for smooth, organic motion (no harsh jitter)
- Scaled or unscaled time — keep shaking while the game is paused
- One-line trigger via a static `Instance`, plus `OnShakeStarted` / `OnShakeStopped` UnityEvents

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/camerashake.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add the **Camera Shake** component to your `Camera` — or to a dedicated empty child "shake pivot" parented to the camera, so other scripts can still move the camera freely.
2. The component captures its local position and rotation at `Awake` as the **rest pose** it shakes around. Make sure the transform is at its neutral pose in the scene.
3. Tune **Max Translation** and **Max Rotation** for how far the camera can move/tilt at full trauma, and **Trauma Decay** for how quickly the shake settles.
4. From gameplay code, call `AddTrauma` (or `Shake`) whenever something impactful happens.
5. Optionally wire **On Shake Started** / **On Shake Stopped** in the Inspector for audio or VFX cues.

### Scripting API

```csharp
using ShahvaizJ.CameraShake;

public class Weapon : MonoBehaviour
{
    private void Fire()
    {
        // Add a punch of trauma on every shot.
        CameraShake.Instance.AddTrauma(0.4f);
    }

    private void Explode()
    {
        // Bigger event, bigger shake.
        CameraShake.Instance.Shake(1f);
    }
}
```

You can also hold a direct reference instead of using `Instance`, and read `Trauma` / `IsShaking` or call `StopImmediate()` to cut the shake instantly.

## License

MIT — see [LICENSE](LICENSE).
