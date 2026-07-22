# Orbit Showcase Camera

An orbiting showcase camera for select-screens with idle auto-rotate, drag-to-orbit, and scroll/pinch zoom.

## Features

- Auto-rotates around the target when idle, resuming after a configurable delay following player input
- Drag to orbit with mouse or single-finger touch — pitch is clamped to a configurable range
- Scroll-wheel zoom on desktop and pinch-to-zoom on mobile, both clamped to a min/max distance
- Smoothly damped yaw, pitch, and distance — no snapping when auto-rotate hands off to manual drag
- `OnDragStarted` / `OnDragEnded` UnityEvents for pausing UI hints or other camera-dependent behavior
- `SetTarget` and `ResetView` API for swapping the showcased item at runtime

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/orbitshowcasecamera.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add an **Orbit Showcase Camera** component to your scene's Camera GameObject.
2. Assign **Target** to the Transform you want to showcase (e.g. a vehicle or character spawn point).
3. Adjust **Target Offset** to aim at the item's body rather than its base.
4. Tune **Orbit**, **Drag**, and **Zoom** settings in the Inspector to taste.
5. Wire **OnDragStarted** / **OnDragEnded** in the Inspector if you want to hide UI hints while the player is orbiting.

### Scripting API

```csharp
using ShahvaizJ.OrbitShowcaseCamera;

OrbitShowcaseCamera showcase = GetComponent<OrbitShowcaseCamera>();

showcase.SetTarget(nextVehicleSpawnPoint);
showcase.ResetView(); // reframe on the new target immediately

bool orbiting = showcase.IsDragging; // read-only
```

## License

MIT — see [LICENSE](LICENSE).
