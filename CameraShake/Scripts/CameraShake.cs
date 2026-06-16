using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.CameraShake
{
    /// <summary>
    /// Trauma-based camera shake driven by Perlin noise. Add trauma when something
    /// impactful happens (a hit, an explosion); trauma decays over time and the shake
    /// amplitude scales with trauma raised to <see cref="_traumaExponent"/>, so big hits
    /// feel punchy while small ones stay subtle.
    /// <para>
    /// Attach this to your camera (or to a dedicated child "shake pivot" so other scripts
    /// can still move the camera freely). The transform's local position and rotation at
    /// <c>Awake</c> are treated as the rest pose that the shake offsets from.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Camera Shake")]
    public class CameraShake : MonoBehaviour
    {
        [Header("Shake Amplitude")]
        [Tooltip("Maximum positional offset (in local units) at full trauma, per axis.")]
        [SerializeField] private Vector3 _maxTranslation = new Vector3(0.3f, 0.3f, 0f);

        [Tooltip("Maximum rotational offset (in degrees) at full trauma, per axis.")]
        [SerializeField] private Vector3 _maxRotation = new Vector3(2f, 2f, 4f);

        [Header("Trauma")]
        [Tooltip("How quickly trauma decays back to zero, in trauma units per second.")]
        [SerializeField] private float _traumaDecay = 1.2f;

        [Tooltip("Shake = trauma ^ exponent. 2-3 keeps small trauma quiet and big trauma loud.")]
        [Range(1f, 4f)]
        [SerializeField] private float _traumaExponent = 2f;

        [Header("Noise")]
        [Tooltip("How fast the noise oscillates. Higher = more frantic shaking.")]
        [SerializeField] private float _frequency = 25f;

        [Tooltip("Use unscaled time so shake keeps animating while the game is paused.")]
        [SerializeField] private bool _useUnscaledTime = false;

        [Header("Events")]
        [Tooltip("Fired once when trauma rises from zero and shaking begins.")]
        public UnityEvent OnShakeStarted;

        [Tooltip("Fired once when trauma reaches zero and shaking stops.")]
        public UnityEvent OnShakeStopped;

        /// <summary>Most recently created instance, for convenient global access.</summary>
        public static CameraShake Instance { get; private set; }

        /// <summary>Current trauma level, clamped to the 0–1 range.</summary>
        public float Trauma => _trauma;

        /// <summary>True while there is any trauma left and the camera is shaking.</summary>
        public bool IsShaking => _trauma > 0f;

        private float _trauma;
        private Vector3 _restPosition;
        private Quaternion _restRotation;

        // Independent noise channels keep each axis from moving in lockstep.
        private float _seedX;
        private float _seedY;
        private float _seedZ;
        private float _seedPitch;
        private float _seedYaw;
        private float _seedRoll;

        private void Awake()
        {
            Instance = this;

            _restPosition = transform.localPosition;
            _restRotation = transform.localRotation;

            _seedX = Random.value * 1000f;
            _seedY = Random.value * 1000f;
            _seedZ = Random.value * 1000f;
            _seedPitch = Random.value * 1000f;
            _seedYaw = Random.value * 1000f;
            _seedRoll = Random.value * 1000f;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Adds trauma to the camera. Call this on impacts — the amount determines how
        /// violent the resulting shake is. Trauma is clamped so it never exceeds 1.
        /// </summary>
        /// <param name="amount">Trauma to add, typically between 0 and 1.</param>
        public void AddTrauma(float amount)
        {
            bool wasIdle = _trauma <= 0f;
            _trauma = Mathf.Clamp01(_trauma + amount);

            if (wasIdle && _trauma > 0f)
                OnShakeStarted.Invoke();
        }

        /// <summary>Sets trauma directly to an absolute value (clamped to 0–1).</summary>
        /// <param name="value">The trauma level to set.</param>
        public void SetTrauma(float value)
        {
            bool wasIdle = _trauma <= 0f;
            _trauma = Mathf.Clamp01(value);

            if (wasIdle && _trauma > 0f)
                OnShakeStarted.Invoke();
            else if (!wasIdle && _trauma <= 0f)
                StopImmediate();
        }

        /// <summary>
        /// Convenience wrapper around <see cref="AddTrauma"/>, reading naturally at call sites
        /// (e.g. <c>CameraShake.Instance.Shake(0.5f)</c>).
        /// </summary>
        /// <param name="amount">Trauma to add, typically between 0 and 1.</param>
        public void Shake(float amount) => AddTrauma(amount);

        /// <summary>Clears all trauma immediately and snaps the camera back to its rest pose.</summary>
        public void StopImmediate()
        {
            bool wasShaking = _trauma > 0f;
            _trauma = 0f;
            transform.localPosition = _restPosition;
            transform.localRotation = _restRotation;

            if (wasShaking)
                OnShakeStopped.Invoke();
        }

        private void LateUpdate()
        {
            if (_trauma <= 0f)
                return;

            float deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float time = (_useUnscaledTime ? Time.unscaledTime : Time.time) * _frequency;

            float shake = Mathf.Pow(_trauma, _traumaExponent);

            Vector3 translationOffset = new Vector3(
                _maxTranslation.x * shake * SignedNoise(_seedX, time),
                _maxTranslation.y * shake * SignedNoise(_seedY, time),
                _maxTranslation.z * shake * SignedNoise(_seedZ, time));

            Vector3 rotationOffset = new Vector3(
                _maxRotation.x * shake * SignedNoise(_seedPitch, time),
                _maxRotation.y * shake * SignedNoise(_seedYaw, time),
                _maxRotation.z * shake * SignedNoise(_seedRoll, time));

            transform.localPosition = _restPosition + translationOffset;
            transform.localRotation = _restRotation * Quaternion.Euler(rotationOffset);

            _trauma = Mathf.Clamp01(_trauma - _traumaDecay * deltaTime);

            if (_trauma <= 0f)
            {
                transform.localPosition = _restPosition;
                transform.localRotation = _restRotation;
                OnShakeStopped.Invoke();
            }
        }

        // Perlin noise is in 0–1; remap to -1..1 so the shake swings both ways around rest.
        private static float SignedNoise(float seed, float time)
        {
            return Mathf.PerlinNoise(seed, time) * 2f - 1f;
        }
    }
}
