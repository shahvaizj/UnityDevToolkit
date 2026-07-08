using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.WaveSpawner
{
    /// <summary>
    /// Drives a sequence of timed enemy/unit waves. Configure each <see cref="WaveEntry"/>
    /// with a prefab pool, count, and spawn interval, then call <see cref="StartWaves"/>.
    /// <para>
    /// Pairs well with an object pool (e.g. ObjectPooler) — subscribe to
    /// <see cref="OnUnitSpawned"/> and swap the default <c>Instantiate</c> call for a
    /// pool <c>Get</c> in your own listener if you want pooled spawns.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Wave Spawner")]
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Waves")]
        [Tooltip("Waves are run in list order, top to bottom.")]
        [SerializeField] private List<WaveEntry> _waves = new List<WaveEntry>();

        [Tooltip("If true, the wave sequence restarts from the first wave after the last one completes.")]
        [SerializeField] private bool _loop;

        [Header("Spawn Points")]
        [Tooltip("Transforms units are spawned at. Required — at least one must be assigned.")]
        [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

        [Tooltip("How a spawn point is chosen for each unit.")]
        [SerializeField] private SpawnPointMode _spawnPointMode = SpawnPointMode.Sequential;

        [Header("Startup")]
        [Tooltip("If true, the wave sequence begins automatically on Start.")]
        [SerializeField] private bool _autoStart = true;

        [Header("Events")]
        [Tooltip("Fired when a wave begins. Parameter is the wave index.")]
        public UnityEvent<int> OnWaveStarted;

        [Tooltip("Fired when a wave finishes spawning all of its units.")]
        public UnityEvent<int> OnWaveCompleted;

        [Tooltip("Fired once after the final wave completes (per loop cycle if looping).")]
        public UnityEvent OnAllWavesCompleted;

        [Tooltip("Fired every time a unit is spawned. Parameter is the spawned instance.")]
        public UnityEvent<GameObject> OnUnitSpawned;

        private Coroutine _runningRoutine;
        private int _nextSpawnPointIndex;

        /// <summary>Index of the wave currently spawning, or -1 if not running.</summary>
        public int CurrentWaveIndex { get; private set; } = -1;

        /// <summary>Number of units still to be spawned in the current wave.</summary>
        public int RemainingInWave { get; private set; }

        /// <summary>True while a wave sequence is actively running.</summary>
        public bool IsRunning => _runningRoutine != null;

        private void Start()
        {
            if (_autoStart)
                StartWaves();
        }

        /// <summary>
        /// Begins the wave sequence from the first wave. Restarts from the top if already running.
        /// </summary>
        public void StartWaves()
        {
            if (_waves.Count == 0)
            {
                Debug.LogWarning("[WaveSpawner] No waves configured — nothing to run.", this);
                return;
            }

            if (_spawnPoints.Count == 0)
            {
                Debug.LogWarning("[WaveSpawner] No spawn points assigned — nothing to run.", this);
                return;
            }

            StopWaves();
            _runningRoutine = StartCoroutine(RunWaves());
        }

        /// <summary>
        /// Stops the wave sequence immediately. Units already spawned are left as-is.
        /// </summary>
        public void StopWaves()
        {
            if (_runningRoutine == null)
                return;

            StopCoroutine(_runningRoutine);
            _runningRoutine = null;
            CurrentWaveIndex = -1;
            RemainingInWave = 0;
        }

        /// <summary>
        /// Stops the current sequence and restarts spawning from the given wave index.
        /// </summary>
        /// <param name="waveIndex">Index into the configured waves list.</param>
        public void SkipToWave(int waveIndex)
        {
            if (waveIndex < 0 || waveIndex >= _waves.Count)
            {
                Debug.LogWarning($"[WaveSpawner] Wave index {waveIndex} is out of range.", this);
                return;
            }

            StopWaves();
            _runningRoutine = StartCoroutine(RunWaves(waveIndex));
        }

        private IEnumerator RunWaves(int startIndex = 0)
        {
            int index = startIndex;

            do
            {
                for (; index < _waves.Count; index++)
                {
                    yield return RunSingleWave(index);
                }

                OnAllWavesCompleted.Invoke();
                index = 0;
            }
            while (_loop);

            _runningRoutine = null;
            CurrentWaveIndex = -1;
        }

        private IEnumerator RunSingleWave(int index)
        {
            WaveEntry wave = _waves[index];
            CurrentWaveIndex = index;
            RemainingInWave = wave.Count;

            if (wave.StartDelay > 0f)
                yield return new WaitForSeconds(wave.StartDelay);

            OnWaveStarted.Invoke(index);

            for (int i = 0; i < wave.Count; i++)
            {
                SpawnUnit(wave);
                RemainingInWave = wave.Count - i - 1;

                if (i < wave.Count - 1)
                {
                    float progress = (float)i / wave.Count;
                    float scale = Mathf.Max(0.01f, wave.IntervalCurve.Evaluate(progress));
                    yield return new WaitForSeconds(wave.SpawnInterval * scale);
                }
            }

            OnWaveCompleted.Invoke(index);
        }

        private void SpawnUnit(WaveEntry wave)
        {
            GameObject prefab = wave.GetRandomPrefab();
            if (prefab == null)
            {
                Debug.LogWarning($"[WaveSpawner] Wave \"{wave.WaveName}\" has no prefabs assigned — skipping spawn.", this);
                return;
            }

            Transform point = GetNextSpawnPoint();
            GameObject instance = Instantiate(prefab, point.position, point.rotation);
            OnUnitSpawned.Invoke(instance);
        }

        private Transform GetNextSpawnPoint()
        {
            if (_spawnPointMode == SpawnPointMode.Random)
                return _spawnPoints[Random.Range(0, _spawnPoints.Count)];

            Transform point = _spawnPoints[_nextSpawnPointIndex];
            _nextSpawnPointIndex = (_nextSpawnPointIndex + 1) % _spawnPoints.Count;
            return point;
        }
    }
}
