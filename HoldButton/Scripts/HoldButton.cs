using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShahvaizJ.HoldButton
{
    public class HoldButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private float _holdDuration = 1f;
        [SerializeField] [Range(0f, 1f)] private float _cancelThreshold = 0.1f;

        [Header("Visual Feedback")]
        [SerializeField] private Graphic _targetGraphic;
        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private Color _progressColor = Color.white;
        [SerializeField] private Color _completeColor = Color.green;

        [Header("Events")]
        public UnityEvent OnHoldStarted;
        public UnityEvent OnHoldComplete;
        public UnityEvent OnHoldCancel;
        public UnityEvent<float> OnHoldProgress;

        public float Progress => _progress;
        public bool IsHolding => _isHolding;
        public Button Button => _button;

        private float _progress;
        private float _holdTimer;
        private bool _isHolding;
        private bool _didComplete;
        private Material _material;
        private bool _initialized;

        private void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            if (_button == null)
            {
                Debug.LogError("HoldButton requires a Button reference. Assign one in the Inspector.", this);
                return;
            }

            if (_targetRenderer != null)
                _material = _targetRenderer.material;

            var trigger = _button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = _button.gameObject.AddComponent<EventTrigger>();

            AddTriggerEntry(trigger, EventTriggerType.PointerDown, OnPointerDown);
            AddTriggerEntry(trigger, EventTriggerType.PointerUp, OnPointerUp);
            AddTriggerEntry(trigger, EventTriggerType.PointerExit, OnPointerExit);

            _initialized = true;
        }

        private void AddTriggerEntry(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        private void Update()
        {
            if (!_initialized || !_isHolding || _didComplete)
                return;

            _holdTimer += Time.unscaledDeltaTime;
            _progress = Mathf.Clamp01(_holdTimer / _holdDuration);

            OnHoldProgress.Invoke(_progress);
            ApplyVisuals(_progress);

            if (_progress >= 1f)
            {
                _didComplete = true;
                _isHolding = false;
                ApplyVisuals(1f);
                OnHoldComplete.Invoke();
            }
        }

        private void OnPointerDown(BaseEventData eventData)
        {
            _isHolding = true;
            _didComplete = false;
            _holdTimer = 0f;
            _progress = 0f;
            OnHoldStarted.Invoke();
        }

        private void OnPointerUp(BaseEventData eventData)
        {
            if (!_isHolding || _didComplete)
                return;

            _isHolding = false;

            if (_progress >= _cancelThreshold)
            {
                _didComplete = true;
                ApplyVisuals(1f);
                OnHoldComplete.Invoke();
            }
            else
            {
                ResetState();
                OnHoldCancel.Invoke();
            }
        }

        private void OnPointerExit(BaseEventData eventData)
        {
            if (!_isHolding || _didComplete)
                return;

            _isHolding = false;
            ResetState();
            OnHoldCancel.Invoke();
        }

        public void Pause()
        {
            _isHolding = false;
        }

        public void Resume()
        {
            if (!_didComplete && _progress > 0f)
                _isHolding = true;
        }

        public void Cancel()
        {
            if (!_isHolding)
                return;

            _isHolding = false;
            ResetState();
            OnHoldCancel.Invoke();
        }

        private void ResetState()
        {
            _progress = 0f;
            _holdTimer = 0f;
            _didComplete = false;
            ApplyVisuals(0f);
        }

        private void ApplyVisuals(float t)
        {
            var color = Color.Lerp(_progressColor, _completeColor, t);

            if (_targetGraphic != null)
                _targetGraphic.color = color;

            if (_material != null)
                _material.color = color;
        }

        private void OnDestroy()
        {
            if (_material != null)
                Destroy(_material);
        }
    }
}
