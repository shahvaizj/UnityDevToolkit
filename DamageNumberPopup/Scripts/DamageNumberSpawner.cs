using System.Collections.Generic;
using UnityEngine;

namespace ShahvaizJ.DamageNumberPopup
{
    /// <summary>
    /// Pools and spawns <see cref="DamageNumberPopup"/> instances at world-space positions.
    /// Assign a prefab with a <see cref="DamageNumberPopup"/> component (and a TextMeshPro
    /// text object) and call <see cref="Spawn"/> whenever damage or healing should show up
    /// as floating combat text.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Damage Number Spawner")]
    public class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Pool")]
        [Tooltip("Prefab carrying a DamageNumberPopup component. Instantiated and recycled by this spawner.")]
        [SerializeField] private DamageNumberPopup _popupPrefab;
        [Tooltip("Instances created up front on Awake so the first hits don't allocate.")]
        [SerializeField] private int _prewarmCount = 10;
        [Tooltip("Maximum pooled instances kept for reuse. 0 means unbounded — excess instances are destroyed instead of pooled.")]
        [SerializeField] private int _maxSize = 0;

        [Header("Facing")]
        [Tooltip("Popups rotate to match this camera's facing each frame, if 'Face Camera' is enabled on the prefab. Leave empty to skip billboarding.")]
        [SerializeField] private Camera _facingCamera;

        [Header("Spawn Offset")]
        [Tooltip("Added to every spawn position, e.g. to lift numbers above a character's head.")]
        [SerializeField] private Vector3 _spawnOffset = new Vector3(0f, 1.5f, 0f);

        private readonly Queue<DamageNumberPopup> _available = new Queue<DamageNumberPopup>();
        private readonly List<DamageNumberPopup> _active = new List<DamageNumberPopup>();

        /// <summary>Number of popups currently mid-flight.</summary>
        public int ActiveCount => _active.Count;

        private void Awake()
        {
            PreWarm();
        }

        /// <summary>
        /// Spawns a popup at <paramref name="worldPosition"/> (plus the configured spawn offset)
        /// showing <paramref name="amount"/>, styled as a critical hit if requested.
        /// </summary>
        /// <param name="worldPosition">World-space origin, typically the hit location.</param>
        /// <param name="amount">The numeric value to display.</param>
        /// <param name="isCrit">Whether to apply critical-hit styling.</param>
        /// <param name="formatOverride">If set, displayed verbatim instead of the formatted <paramref name="amount"/> (e.g. "MISS").</param>
        /// <returns>The spawned popup, or null if no prefab is assigned.</returns>
        public DamageNumberPopup Spawn(Vector3 worldPosition, float amount, bool isCrit = false, string formatOverride = null)
        {
            if (_popupPrefab == null)
            {
                Debug.LogWarning("[DamageNumberSpawner] No popup prefab assigned.", this);
                return null;
            }

            DamageNumberPopup popup = GetFromPool();
            _active.Add(popup);
            popup.Play(worldPosition + _spawnOffset, amount, isCrit, _facingCamera, ReturnToPool, formatOverride);
            return popup;
        }

        private DamageNumberPopup GetFromPool()
        {
            while (_available.Count > 0)
            {
                DamageNumberPopup popup = _available.Dequeue();
                if (popup != null)
                    return popup;
            }

            return CreateInstance();
        }

        private void ReturnToPool(DamageNumberPopup popup)
        {
            _active.Remove(popup);

            if (_maxSize > 0 && _available.Count >= _maxSize)
            {
                Destroy(popup.gameObject);
                return;
            }

            popup.transform.SetParent(transform, false);
            _available.Enqueue(popup);
        }

        private void PreWarm()
        {
            for (int i = 0; i < _prewarmCount; i++)
            {
                DamageNumberPopup popup = CreateInstance();
                popup.gameObject.SetActive(false);
                _available.Enqueue(popup);
            }
        }

        private DamageNumberPopup CreateInstance()
        {
            return Instantiate(_popupPrefab, transform);
        }
    }
}
