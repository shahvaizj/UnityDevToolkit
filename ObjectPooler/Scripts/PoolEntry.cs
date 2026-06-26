using UnityEngine;

namespace ShahvaizJ.ObjectPooler
{
    /// <summary>
    /// Configuration for a single keyed pool. Assign one of these per prefab type
    /// in the <see cref="ObjectPooler"/> Inspector list.
    /// </summary>
    [System.Serializable]
    public class PoolEntry
    {
        [Tooltip("Unique key used to request objects from this pool (e.g. \"Bullet\", \"Coin\").")]
        public string Key;

        [Tooltip("The prefab to instantiate for this pool.")]
        public GameObject Prefab;

        [Tooltip("Number of instances to create at startup.")]
        [Min(0)]
        public int PreWarmCount = 5;

        [Tooltip("Hard cap on pool size. Set to 0 for unlimited growth.")]
        [Min(0)]
        public int MaxSize;

        [Tooltip("If true, new instances are created when the pool is empty and max size has not been reached. If false, Get returns null when the pool is empty.")]
        public bool AutoGrow = true;
    }
}
