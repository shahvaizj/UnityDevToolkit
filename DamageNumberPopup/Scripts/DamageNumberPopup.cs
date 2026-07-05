using System;
using TMPro;
using UnityEngine;

namespace ShahvaizJ.DamageNumberPopup
{
    /// <summary>
    /// A single pooled, world-space floating number. Rides an upward arc under gravity,
    /// fades out near the end of its lifetime, and applies distinct color/scale styling
    /// for critical hits. Spawned and recycled by <see cref="DamageNumberSpawner"/> —
    /// use the spawner's <c>Spawn</c> method rather than calling <see cref="Play"/> directly.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Damage Number Popup")]
    public class DamageNumberPopup : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The text component this popup drives. Auto-fetched from this GameObject if left empty.")]
        [SerializeField] private TMP_Text _label;

        [Header("Arc Motion")]
        [Tooltip("Total seconds the popup stays alive before returning to its pool.")]
        [SerializeField] private float _lifetime = 1f;
        [Tooltip("Initial upward speed in world units per second.")]
        [SerializeField] private float _upwardSpeed = 2.5f;
        [Tooltip("Maximum random sideways speed applied at spawn, in either direction.")]
        [SerializeField] private float _horizontalSpread = 0.6f;
        [Tooltip("Downward acceleration applied every frame, in world units per second squared.")]
        [SerializeField] private float _gravity = 4f;

        [Header("Fade")]
        [Tooltip("Normalized lifetime (0-1) at which the fade-out begins.")]
        [SerializeField, Range(0f, 1f)] private float _fadeStartNormalized = 0.5f;

        [Header("Crit Styling")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _critColor = new Color(1f, 0.65f, 0f);
        [Tooltip("Multiplier applied to this transform's base local scale for critical hits.")]
        [SerializeField] private float _critScaleMultiplier = 1.4f;
        [Tooltip("Suffix appended to the formatted number on a critical hit (e.g. \"!\").")]
        [SerializeField] private string _critSuffix = "!";

        [Header("Facing")]
        [Tooltip("If true, the popup rotates each frame to match the facing camera supplied by the spawner.")]
        [SerializeField] private bool _faceCamera = true;

        private Vector3 _velocity;
        private Vector3 _baseScale;
        private Camera _facingCamera;
        private Action<DamageNumberPopup> _releaseCallback;
        private float _elapsed;
        private bool _isPlaying;

        private void Awake()
        {
            if (_label == null)
                _label = GetComponent<TMP_Text>();

            _baseScale = transform.localScale;
        }

        private void Update()
        {
            if (!_isPlaying)
                return;

            _elapsed += Time.deltaTime;
            float t = _lifetime > 0f ? Mathf.Clamp01(_elapsed / _lifetime) : 1f;

            _velocity.y -= _gravity * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;

            if (_faceCamera && _facingCamera != null)
                transform.rotation = _facingCamera.transform.rotation;

            ApplyFade(t);

            if (t >= 1f)
                Release();
        }

        /// <summary>
        /// Activates this popup at <paramref name="worldPosition"/> and begins its arc/fade lifecycle.
        /// Called by <see cref="DamageNumberSpawner"/> — prefer <c>DamageNumberSpawner.Spawn</c> in game code.
        /// </summary>
        /// <param name="worldPosition">World-space spawn position, already offset by the caller.</param>
        /// <param name="amount">The numeric value to display.</param>
        /// <param name="isCrit">Whether to apply critical-hit color/scale/suffix styling.</param>
        /// <param name="facingCamera">Camera to billboard toward each frame, or null to skip billboarding.</param>
        /// <param name="releaseCallback">Invoked once the popup finishes, so the caller can return it to its pool.</param>
        /// <param name="formatOverride">If set, displayed verbatim instead of the formatted <paramref name="amount"/>.</param>
        public void Play(Vector3 worldPosition, float amount, bool isCrit, Camera facingCamera, Action<DamageNumberPopup> releaseCallback, string formatOverride = null)
        {
            transform.position = worldPosition;
            transform.localScale = isCrit ? _baseScale * _critScaleMultiplier : _baseScale;

            _facingCamera = facingCamera;
            _releaseCallback = releaseCallback;
            _elapsed = 0f;
            _isPlaying = true;
            _velocity = new Vector3(UnityEngine.Random.Range(-_horizontalSpread, _horizontalSpread), _upwardSpeed, 0f);

            if (_faceCamera && _facingCamera != null)
                transform.rotation = _facingCamera.transform.rotation;

            if (_label != null)
            {
                _label.text = formatOverride ?? FormatAmount(amount, isCrit);
                Color color = isCrit ? _critColor : _normalColor;
                color.a = 1f;
                _label.color = color;
            }

            gameObject.SetActive(true);
        }

        private string FormatAmount(float amount, bool isCrit)
        {
            string number = Mathf.RoundToInt(amount).ToString();
            return isCrit ? number + _critSuffix : number;
        }

        private void ApplyFade(float t)
        {
            if (_label == null)
                return;

            float alpha = t < _fadeStartNormalized
                ? 1f
                : 1f - Mathf.InverseLerp(_fadeStartNormalized, 1f, t);

            Color color = _label.color;
            color.a = alpha;
            _label.color = color;
        }

        private void Release()
        {
            _isPlaying = false;
            gameObject.SetActive(false);
            Action<DamageNumberPopup> callback = _releaseCallback;
            _releaseCallback = null;
            callback?.Invoke(this);
        }
    }
}
