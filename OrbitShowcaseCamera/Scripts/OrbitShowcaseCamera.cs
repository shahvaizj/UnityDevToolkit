using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.OrbitShowcaseCamera
{
    /// <summary>
    /// Orbiting showcase camera for select-screens (vehicle/character/item previews).
    /// Auto-rotates around a target while idle, lets the player drag (mouse or touch)
    /// to orbit manually, and scroll/pinch to zoom. Attach directly to the camera
    /// GameObject you want to drive.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Orbit Showcase Camera")]
    public class OrbitShowcaseCamera : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("Transform the camera orbits around. Can be reassigned at runtime via SetTarget.")]
        [SerializeField] private Transform _target;

        [Tooltip("Look-at point relative to the target, e.g. raise it to aim at a vehicle body instead of the wheels.")]
        [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1f, 0f);

        [Header("Orbit")]
        [SerializeField] private float _startYaw = 0f;
        [SerializeField] private float _startPitch = 15f;
        [Tooltip("Degrees per second while idle.")]
        [SerializeField] private float _autoRotateSpeed = 12f;
        [Tooltip("Seconds after the player stops dragging before auto-rotate resumes.")]
        [SerializeField] private float _autoRotateResumeDelay = 2f;

        [Header("Drag")]
        [SerializeField] private bool _allowDrag = true;
        [Tooltip("Degrees per pixel dragged.")]
        [SerializeField] private float _dragSensitivity = 0.25f;
        [SerializeField] private float _minPitch = 5f;
        [SerializeField] private float _maxPitch = 60f;
        [Tooltip("Higher = snappier follow of drag input.")]
        [SerializeField] private float _rotationSmoothing = 10f;

        [Header("Zoom")]
        [SerializeField] private bool _allowZoom = true;
        [SerializeField] private float _distance = 6f;
        [SerializeField] private float _minDistance = 3f;
        [SerializeField] private float _maxDistance = 12f;
        [SerializeField] private float _scrollZoomSpeed = 3f;
        [SerializeField] private float _pinchZoomSpeed = 0.02f;
        [SerializeField] private float _zoomSmoothing = 10f;

        [Header("Events")]
        public UnityEvent OnDragStarted;
        public UnityEvent OnDragEnded;

        /// <summary>The transform currently being orbited. Assign via SetTarget for a clean reframe.</summary>
        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        /// <summary>True while the player is actively dragging to orbit.</summary>
        public bool IsDragging => _dragging;

        /// <summary>Current camera distance from the target's focus point.</summary>
        public float CurrentDistance => _curDistance;

        private float _yaw;
        private float _pitch;
        private float _targetYaw;
        private float _targetPitch;
        private float _curDistance;
        private float _targetDistance;
        private float _idleTimer;

        private bool _dragging;
        private Vector2 _lastPointerPos;
        private float _lastPinchDist;

        private void OnEnable()
        {
            _targetYaw = _yaw = _startYaw;
            _targetPitch = _pitch = _startPitch;
            _targetDistance = _curDistance = _distance;
            _idleTimer = _autoRotateResumeDelay;
            _dragging = false;

            ApplyTransform();
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            if (_allowDrag) HandleDrag();
            if (_allowZoom) HandleZoom();

            if (!_dragging)
            {
                _idleTimer += Time.deltaTime;
                if (_idleTimer >= _autoRotateResumeDelay)
                    _targetYaw += _autoRotateSpeed * Time.deltaTime;
            }

            _targetPitch = Mathf.Clamp(_targetPitch, _minPitch, _maxPitch);
            _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);

            float rotSmooth = 1f - Mathf.Exp(-_rotationSmoothing * Time.deltaTime);
            _yaw = Mathf.LerpAngle(_yaw, _targetYaw, rotSmooth);
            _pitch = Mathf.Lerp(_pitch, _targetPitch, rotSmooth);

            float zoomSmooth = 1f - Mathf.Exp(-_zoomSmoothing * Time.deltaTime);
            _curDistance = Mathf.Lerp(_curDistance, _targetDistance, zoomSmooth);

            ApplyTransform();
        }

        private void ApplyTransform()
        {
            if (_target == null)
                return;

            Vector3 focus = _target.position + _targetOffset;
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 position = focus - rotation * Vector3.forward * _curDistance;

            transform.SetPositionAndRotation(position, rotation);
        }

        private void HandleDrag()
        {
            // Two fingers means the player is pinching, not dragging.
            if (Input.touchCount >= 2)
            {
                EndDrag();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _dragging = true;
                _lastPointerPos = Input.mousePosition;
                OnDragStarted.Invoke();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }

            if (_dragging && Input.GetMouseButton(0))
            {
                Vector2 pointerPos = Input.mousePosition;
                Vector2 delta = pointerPos - _lastPointerPos;
                _lastPointerPos = pointerPos;

                _targetYaw += delta.x * _dragSensitivity;
                _targetPitch -= delta.y * _dragSensitivity;
            }
        }

        private void EndDrag()
        {
            if (!_dragging)
                return;

            _dragging = false;
            _idleTimer = 0f;
            OnDragEnded.Invoke();
        }

        private void HandleZoom()
        {
            // Mouse scroll wheel (editor / desktop).
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (!Mathf.Approximately(scroll, 0f))
            {
                _targetDistance -= scroll * _scrollZoomSpeed;
                _idleTimer = 0f;
            }

            // Pinch (mobile).
            if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);
                float pinchDist = Vector2.Distance(t0.position, t1.position);

                if (t1.phase == TouchPhase.Began)
                    _lastPinchDist = pinchDist;

                _targetDistance -= (pinchDist - _lastPinchDist) * _pinchZoomSpeed;
                _lastPinchDist = pinchDist;
                _idleTimer = 0f;
            }
        }

        /// <summary>
        /// Reassigns the orbit target at runtime. Pass <c>null</c> to stop orbiting.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// Resets yaw, pitch, and distance back to their configured starting values
        /// and restarts the idle timer. Call when switching between showcased items.
        /// </summary>
        public void ResetView()
        {
            _targetYaw = _startYaw;
            _targetPitch = _startPitch;
            _targetDistance = _distance;
            _idleTimer = _autoRotateResumeDelay;
        }
    }
}
