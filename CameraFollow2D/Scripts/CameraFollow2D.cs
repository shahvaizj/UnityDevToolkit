using UnityEngine;

namespace ShahvaizJ.CameraFollow2D
{
    /// <summary>
    /// Smooth-damped 2D camera follow with a configurable deadzone and optional world-bounds
    /// clamping. Attach to the camera itself (a <see cref="Camera"/> component is required so
    /// bounds clamping can account for the visible viewport). The target only pushes the camera
    /// once it leaves the deadzone rectangle, giving a stable, non-jittery follow feel.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Camera Follow 2D")]
    [RequireComponent(typeof(Camera))]
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("Transform the camera follows. Can be reassigned at runtime via SetTarget.")]
        [SerializeField] private Transform _target;

        [Tooltip("Constant world-space offset applied to the followed position (e.g. keep Z at -10).")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

        [Header("Deadzone")]
        [Tooltip("Width/height of the deadzone box, centered on the camera, that the target can move within before the camera starts following.")]
        [SerializeField] private Vector2 _deadzoneSize = new Vector2(2f, 1.5f);

        [Header("Smoothing")]
        [Tooltip("Approximate time to reach the desired position. Smaller values follow more tightly.")]
        [SerializeField] private float _smoothTime = 0.2f;

        [Tooltip("Maximum speed the camera can move at while smoothing, in world units per second.")]
        [SerializeField] private float _maxSpeed = 25f;

        [Tooltip("Follow using unscaled time so the camera keeps tracking while Time.timeScale is 0.")]
        [SerializeField] private bool _useUnscaledTime = false;

        [Header("World Bounds")]
        [Tooltip("Clamp the camera so its viewport never shows outside the configured world bounds.")]
        [SerializeField] private bool _useBounds = false;

        [Tooltip("Bottom-left corner of the allowed world area. Only used when Use Bounds is on.")]
        [SerializeField] private Vector2 _minBounds = new Vector2(-10f, -10f);

        [Tooltip("Top-right corner of the allowed world area. Only used when Use Bounds is on.")]
        [SerializeField] private Vector2 _maxBounds = new Vector2(10f, 10f);

        /// <summary>The transform currently being followed.</summary>
        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private Camera _camera;
        private Vector3 _velocity;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            float deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            Vector2 currentPos = transform.position;
            Vector2 targetPos = _target.position + _offset;
            Vector2 delta = targetPos - currentPos;

            Vector2 desiredPos = currentPos;

            if (Mathf.Abs(delta.x) > _deadzoneSize.x * 0.5f)
                desiredPos.x = targetPos.x - Mathf.Sign(delta.x) * _deadzoneSize.x * 0.5f;

            if (Mathf.Abs(delta.y) > _deadzoneSize.y * 0.5f)
                desiredPos.y = targetPos.y - Mathf.Sign(delta.y) * _deadzoneSize.y * 0.5f;

            if (_useBounds)
                desiredPos = ClampToBounds(desiredPos);

            Vector3 desiredPos3D = new Vector3(desiredPos.x, desiredPos.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos3D, ref _velocity, _smoothTime, _maxSpeed, deltaTime);
        }

        /// <summary>
        /// Clamps a candidate camera position so the visible viewport stays fully within
        /// <see cref="_minBounds"/>/<see cref="_maxBounds"/>. Accounts for the orthographic
        /// viewport size so the world edges never show past the configured bounds.
        /// </summary>
        private Vector2 ClampToBounds(Vector2 position)
        {
            if (!_camera.orthographic)
                return Vector2.Max(_minBounds, Vector2.Min(_maxBounds, position));

            float halfHeight = _camera.orthographicSize;
            float halfWidth = halfHeight * _camera.aspect;

            float minX = _minBounds.x + halfWidth;
            float maxX = _maxBounds.x - halfWidth;
            float minY = _minBounds.y + halfHeight;
            float maxY = _maxBounds.y - halfHeight;

            // If the bounds are smaller than the viewport, hold the camera centered on that axis.
            position.x = minX > maxX ? (_minBounds.x + _maxBounds.x) * 0.5f : Mathf.Clamp(position.x, minX, maxX);
            position.y = minY > maxY ? (_minBounds.y + _maxBounds.y) * 0.5f : Mathf.Clamp(position.y, minY, maxY);

            return position;
        }

        /// <summary>
        /// Reassigns the follow target at runtime. Pass <c>null</c> to stop following.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// Updates the world-space bounds used for clamping and enables bounds clamping.
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            _minBounds = min;
            _maxBounds = max;
            _useBounds = true;
        }

        /// <summary>
        /// Instantly moves the camera to the target's position (plus offset), bypassing
        /// smoothing. Useful after teleports, respawns, or scene transitions.
        /// </summary>
        public void SnapToTarget()
        {
            if (_target == null)
                return;

            Vector3 snapPos = _target.position + _offset;

            if (_useBounds)
            {
                Vector2 clamped = ClampToBounds(snapPos);
                snapPos = new Vector3(clamped.x, clamped.y, snapPos.z);
            }

            transform.position = snapPos;
            _velocity = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(_deadzoneSize.x, _deadzoneSize.y, 0f));

            if (_useBounds)
            {
                Gizmos.color = Color.cyan;
                Vector3 center = new Vector3((_minBounds.x + _maxBounds.x) * 0.5f, (_minBounds.y + _maxBounds.y) * 0.5f, transform.position.z);
                Vector3 size = new Vector3(_maxBounds.x - _minBounds.x, _maxBounds.y - _minBounds.y, 0f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
