# Wave Spawner

Timed/wave-based enemy spawner with per-wave counts, intervals, spawn-rate curves, and looping.

## Features

- Ordered list of waves, each with its own prefab pool, unit count, and spawn interval
- Per-wave `AnimationCurve` scales the spawn interval across wave progress — ramp spawns up or down over time
- Sequential or random spawn point selection across any number of `Transform` points
- Optional looping back to the first wave after the last one completes
- `StartWaves`, `StopWaves`, and `SkipToWave` for full runtime control
- `OnWaveStarted`, `OnWaveCompleted`, `OnAllWavesCompleted`, and `OnUnitSpawned` UnityEvents for UI, audio, and gameplay hooks
- Pairs naturally with an object pool — hook `OnUnitSpawned` for pooled-spawn integration

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/wavespawner.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add the **Wave Spawner** component to an empty GameObject (e.g. "Spawner").
2. In the **Waves** list, click **+** to add a wave. Set a **Wave Name**, assign one or more **Prefabs**, and set **Count**, **Spawn Interval**, and **Start Delay**.
3. Optionally edit the **Interval Curve** to speed up or slow down spawns as the wave progresses (flat = constant rate).
4. Assign one or more **Spawn Points** and choose **Sequential** or **Random** in **Spawn Point Mode**.
5. Leave **Auto Start** on to begin spawning on `Start`, or call `StartWaves()` from your own game-start logic.

### Scripting API

```csharp
using UnityEngine;
using ShahvaizJ.WaveSpawner;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private WaveSpawner _spawner;

    private void OnEnable()
    {
        _spawner.OnWaveStarted.AddListener(OnWaveStarted);
        _spawner.OnAllWavesCompleted.AddListener(OnAllWavesCompleted);
    }

    private void OnWaveStarted(int waveIndex)
    {
        Debug.Log($"Wave {waveIndex + 1} started!");
    }

    private void OnAllWavesCompleted()
    {
        Debug.Log("All waves cleared!");
    }
}
```

## License

MIT — see [LICENSE](LICENSE).
