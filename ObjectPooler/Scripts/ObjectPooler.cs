using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.ObjectPooler
{
    /// <summary>
    /// A keyed object pool manager. Register prefabs with string keys in the Inspector,
    /// then call <see cref="Get"/> and <see cref="Return"/> at runtime instead of
    /// <c>Instantiate</c> / <c>Destroy</c>.
    /// <para>
    /// Each pool can pre-warm a configurable number of instances on <c>Awake</c>,
    /// auto-grow when empty, and enforce an optional max size. Objects that implement
    /// <see cref="IPoolable"/> receive <c>OnSpawnFromPool</c> and <c>OnReturnToPool</c>
    /// callbacks for state reset.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Object Pooler")]
    public class ObjectPooler : MonoBehaviour
    {
        [Header("Pool Configuration")]
        [Tooltip("List of pools, each identified by a unique key.")]
        [SerializeField] private List<PoolEntry> _pools = new List<PoolEntry>();

        [Header("Organisation")]
        [Tooltip("If true, pooled objects are parented under this transform to keep the hierarchy tidy.")]
        [SerializeField] private bool _parentToSelf = true;

        [Header("Events")]
        [Tooltip("Fired when an object is spawned from a pool. Parameter is the spawned GameObject.")]
        public UnityEvent<GameObject> OnObjectSpawned;

        [Tooltip("Fired when an object is returned to a pool. Parameter is the returned GameObject.")]
        public UnityEvent<GameObject> OnObjectReturned;

        private readonly Dictionary<string, Queue<GameObject>> _available = new Dictionary<string, Queue<GameObject>>();
        private readonly Dictionary<string, PoolEntry> _entryLookup = new Dictionary<string, PoolEntry>();
        private readonly Dictionary<GameObject, string> _activeObjects = new Dictionary<GameObject, string>();

        /// <summary>Number of registered pool keys.</summary>
        public int PoolCount => _entryLookup.Count;

        private void Awake()
        {
            InitialisePools();
        }

        /// <summary>
        /// Retrieves an object from the pool identified by <paramref name="key"/>,
        /// positions it, activates it, and fires any <see cref="IPoolable"/> callbacks.
        /// Returns <c>null</c> if the pool is empty and cannot grow.
        /// </summary>
        /// <param name="key">The pool key set in the Inspector.</param>
        /// <param name="position">World position for the spawned object.</param>
        /// <param name="rotation">World rotation for the spawned object.</param>
        /// <returns>The activated <see cref="GameObject"/>, or <c>null</c> if unavailable.</returns>
        public GameObject Get(string key, Vector3 position, Quaternion rotation)
        {
            if (!_entryLookup.TryGetValue(key, out PoolEntry entry))
            {
                Debug.LogWarning($"[ObjectPooler] No pool registered with key \"{key}\".", this);
                return null;
            }

            Queue<GameObject> queue = _available[key];
            GameObject instance = null;

            while (queue.Count > 0)
            {
                instance = queue.Dequeue();
                if (instance != null)
                    break;
                instance = null;
            }

            if (instance == null)
            {
                if (!entry.AutoGrow)
                    return null;

                if (entry.MaxSize > 0 && CountActive(key) >= entry.MaxSize)
                    return null;

                instance = CreateInstance(entry);
            }

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            _activeObjects[instance] = key;

            NotifySpawn(instance);
            OnObjectSpawned.Invoke(instance);

            return instance;
        }

        /// <summary>
        /// Retrieves an object from the pool at the default position and rotation.
        /// </summary>
        /// <param name="key">The pool key set in the Inspector.</param>
        /// <returns>The activated <see cref="GameObject"/>, or <c>null</c> if unavailable.</returns>
        public GameObject Get(string key)
        {
            return Get(key, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Deactivates and returns an object to its originating pool. Fires any
        /// <see cref="IPoolable.OnReturnToPool"/> callbacks.
        /// </summary>
        /// <param name="instance">The object to return. Must have been obtained via <see cref="Get"/>.</param>
        public void Return(GameObject instance)
        {
            if (instance == null)
                return;

            if (!_activeObjects.TryGetValue(instance, out string key))
            {
                Debug.LogWarning($"[ObjectPooler] Object \"{instance.name}\" was not spawned by this pooler.", this);
                return;
            }

            NotifyReturn(instance);
            instance.SetActive(false);

            if (_parentToSelf)
                instance.transform.SetParent(transform, false);

            _activeObjects.Remove(instance);
            _available[key].Enqueue(instance);

            OnObjectReturned.Invoke(instance);
        }

        /// <summary>
        /// Returns all currently active objects across every pool.
        /// </summary>
        public void ReturnAll()
        {
            var snapshot = new List<GameObject>(_activeObjects.Keys);
            foreach (GameObject obj in snapshot)
            {
                Return(obj);
            }
        }

        /// <summary>
        /// Returns all active objects belonging to the specified pool key.
        /// </summary>
        /// <param name="key">The pool key.</param>
        public void ReturnAll(string key)
        {
            var toReturn = new List<GameObject>();
            foreach (var kvp in _activeObjects)
            {
                if (kvp.Value == key)
                    toReturn.Add(kvp.Key);
            }

            foreach (GameObject obj in toReturn)
            {
                Return(obj);
            }
        }

        /// <summary>
        /// Returns the number of inactive (available) objects in the specified pool.
        /// </summary>
        /// <param name="key">The pool key.</param>
        /// <returns>Count of available instances, or -1 if the key is not registered.</returns>
        public int CountAvailable(string key)
        {
            return _available.TryGetValue(key, out Queue<GameObject> queue) ? queue.Count : -1;
        }

        /// <summary>
        /// Returns the number of currently active (checked-out) objects from the specified pool.
        /// </summary>
        /// <param name="key">The pool key.</param>
        /// <returns>Count of active instances.</returns>
        public int CountActive(string key)
        {
            int count = 0;
            foreach (var kvp in _activeObjects)
            {
                if (kvp.Value == key)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Checks whether a pool with the given key is registered.
        /// </summary>
        /// <param name="key">The pool key to check.</param>
        /// <returns><c>true</c> if the key exists.</returns>
        public bool HasPool(string key)
        {
            return _entryLookup.ContainsKey(key);
        }

        /// <summary>
        /// Adds instances to an existing pool at runtime. Useful for topping up a pool
        /// during a loading screen or quiet moment.
        /// </summary>
        /// <param name="key">The pool key to top up.</param>
        /// <param name="count">Number of instances to add.</param>
        public void WarmUp(string key, int count)
        {
            if (!_entryLookup.TryGetValue(key, out PoolEntry entry))
            {
                Debug.LogWarning($"[ObjectPooler] Cannot warm up — no pool with key \"{key}\".", this);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (entry.MaxSize > 0 && _available[key].Count + CountActive(key) >= entry.MaxSize)
                    break;

                GameObject instance = CreateInstance(entry);
                instance.SetActive(false);
                _available[key].Enqueue(instance);
            }
        }

        /// <summary>
        /// Registers a new pool at runtime. Skips registration if the key already exists.
        /// </summary>
        /// <param name="entry">Pool configuration to register.</param>
        public void RegisterPool(PoolEntry entry)
        {
            if (string.IsNullOrEmpty(entry.Key) || entry.Prefab == null)
            {
                Debug.LogWarning("[ObjectPooler] Cannot register pool — key or prefab is missing.", this);
                return;
            }

            if (_entryLookup.ContainsKey(entry.Key))
            {
                Debug.LogWarning($"[ObjectPooler] Pool key \"{entry.Key}\" is already registered.", this);
                return;
            }

            _entryLookup[entry.Key] = entry;
            _available[entry.Key] = new Queue<GameObject>();
            PreWarm(entry);
        }

        private void InitialisePools()
        {
            foreach (PoolEntry entry in _pools)
            {
                if (string.IsNullOrEmpty(entry.Key) || entry.Prefab == null)
                {
                    Debug.LogWarning("[ObjectPooler] Skipping pool entry with missing key or prefab.", this);
                    continue;
                }

                if (_entryLookup.ContainsKey(entry.Key))
                {
                    Debug.LogWarning($"[ObjectPooler] Duplicate pool key \"{entry.Key}\" — skipping.", this);
                    continue;
                }

                _entryLookup[entry.Key] = entry;
                _available[entry.Key] = new Queue<GameObject>();
                PreWarm(entry);
            }
        }

        private void PreWarm(PoolEntry entry)
        {
            for (int i = 0; i < entry.PreWarmCount; i++)
            {
                GameObject instance = CreateInstance(entry);
                instance.SetActive(false);
                _available[entry.Key].Enqueue(instance);
            }
        }

        private GameObject CreateInstance(PoolEntry entry)
        {
            GameObject instance = Instantiate(entry.Prefab);
            instance.name = $"{entry.Prefab.name} [{entry.Key}]";

            if (_parentToSelf)
                instance.transform.SetParent(transform, false);

            return instance;
        }

        private static void NotifySpawn(GameObject instance)
        {
            var poolables = instance.GetComponents<IPoolable>();
            foreach (IPoolable poolable in poolables)
            {
                poolable.OnSpawnFromPool();
            }
        }

        private static void NotifyReturn(GameObject instance)
        {
            var poolables = instance.GetComponents<IPoolable>();
            foreach (IPoolable poolable in poolables)
            {
                poolable.OnReturnToPool();
            }
        }
    }
}
