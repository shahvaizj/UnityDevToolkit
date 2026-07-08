using System.Collections.Generic;
using UnityEngine;

namespace ShahvaizJ.WaveSpawner
{
    /// <summary>
    /// Configuration for a single wave. Assign one of these per wave in the
    /// <see cref="WaveSpawner"/> Inspector list.
    /// </summary>
    [System.Serializable]
    public class WaveEntry
    {
        [Tooltip("Label shown in logs and the Inspector list (purely cosmetic).")]
        public string WaveName = "Wave";

        [Tooltip("Prefabs this wave can spawn. One is picked at random for each unit.")]
        public List<GameObject> Prefabs = new List<GameObject>();

        [Tooltip("Total number of units to spawn during this wave.")]
        [Min(1)]
        public int Count = 5;

        [Tooltip("Base seconds between individual spawns within this wave.")]
        [Min(0.01f)]
        public float SpawnInterval = 1f;

        [Tooltip("Seconds to wait after the previous wave ends before this wave's first spawn.")]
        [Min(0f)]
        public float StartDelay = 2f;

        [Tooltip("Scales SpawnInterval across wave progress (0 = wave start, 1 = wave end). " +
                 "A flat line at 1 keeps a constant rate; a falling curve speeds spawns up over time.")]
        public AnimationCurve IntervalCurve = AnimationCurve.Constant(0f, 1f, 1f);

        /// <summary>
        /// Picks a random prefab from <see cref="Prefabs"/>. Returns <c>null</c> if the list is empty.
        /// </summary>
        public GameObject GetRandomPrefab()
        {
            if (Prefabs == null || Prefabs.Count == 0)
                return null;

            return Prefabs[Random.Range(0, Prefabs.Count)];
        }
    }
}
