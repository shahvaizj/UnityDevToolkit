using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ShahvaizJ.InputRebindUI
{
    /// <summary>
    /// Drives a single rebindable-control row: starts an interactive rebind for one action
    /// binding, shows the current binding as text, and can revert the binding to its default.
    /// Assign one of these per row in your rebinding menu, pointing at the action and binding
    /// index that row controls (binding index 0 for a simple binding, or the composite part
    /// index — e.g. up/down/left/right of a 2D Vector composite — for composite bindings).
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Input Rebind UI/Rebind Action UI")]
    public class RebindActionUI : MonoBehaviour
    {
        [Header("Action")]
        [Tooltip("The action whose binding this row controls.")]
        [SerializeField] private InputActionReference _actionReference;

        [Tooltip("Index into the action's bindings list. Use 0 for a simple binding, or the composite part index for composite bindings (e.g. WASD).")]
        [SerializeField] private int _bindingIndex;

        [Header("UI")]
        [Tooltip("Optional label showing the action's display name.")]
        [SerializeField] private Text _actionNameText;

        [Tooltip("Text updated with the current binding's display string.")]
        [SerializeField] private Text _bindingDisplayText;

        [Tooltip("Button that starts an interactive rebind when clicked.")]
        [SerializeField] private Button _rebindButton;

        [Tooltip("Optional GameObject shown only while waiting for input (e.g. a 'Press any key' overlay).")]
        [SerializeField] private GameObject _waitingForInputIndicator;

        [Header("Rebind Behaviour")]
        [Tooltip("Control paths ignored while listening, so incidental input (e.g. mouse movement) can't be captured as a binding.")]
        [SerializeField] private string[] _excludedControlPaths = { "Mouse/Position", "Mouse/Delta" };

        [Tooltip("Control path that cancels the rebind when actuated.")]
        [SerializeField] private string _cancelControlPath = "<Keyboard>/escape";

        [Tooltip("Seconds to keep listening after the first suitable control is actuated, in case a better match (e.g. a held modifier) follows.")]
        [SerializeField] private float _matchWaitSeconds = 0.05f;

        [Header("Events")]
        [Tooltip("Fired when an interactive rebind begins.")]
        public UnityEvent OnRebindStarted;

        [Tooltip("Fired after a rebind completes and the new binding has been applied.")]
        public UnityEvent OnRebindComplete;

        [Tooltip("Fired when a rebind is canceled without changing the binding.")]
        public UnityEvent OnRebindCanceled;

        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        private void Awake()
        {
            if (_rebindButton != null)
                _rebindButton.onClick.AddListener(StartRebind);
        }

        private void Start()
        {
            UpdateBindingDisplay();
        }

        private void OnDestroy()
        {
            _rebindOperation?.Dispose();
        }

        /// <summary>
        /// Begins an interactive rebind for this row's action/binding. The bound action is
        /// disabled while listening and re-enabled once the rebind completes or is canceled.
        /// </summary>
        public void StartRebind()
        {
            InputAction action = ResolveAction();
            if (action == null)
                return;

            _rebindOperation?.Dispose();
            action.Disable();

            InputActionRebindingExtensions.RebindingOperation rebind = action
                .PerformInteractiveRebinding(_bindingIndex)
                .WithCancelingThrough(_cancelControlPath)
                .OnMatchWaitForAnother(_matchWaitSeconds)
                .OnComplete(operation => FinishRebind(action, operation))
                .OnCancel(operation => CancelRebind(action, operation));

            foreach (string path in _excludedControlPaths)
                rebind.WithControlsExcluding(path);

            if (_waitingForInputIndicator != null)
                _waitingForInputIndicator.SetActive(true);

            OnRebindStarted.Invoke();

            _rebindOperation = rebind;
            _rebindOperation.Start();
        }

        /// <summary>
        /// Removes any override on this row's binding, reverting it to the action's default,
        /// refreshes the display text, and saves the change via the scene's
        /// <see cref="InputRebindManager"/> if one is present.
        /// </summary>
        public void ResetBinding()
        {
            InputAction action = ResolveAction();
            if (action == null)
                return;

            action.RemoveBindingOverride(_bindingIndex);
            UpdateBindingDisplay();
            InputRebindManager.Instance?.SaveBindings();
        }

        /// <summary>Refreshes the binding display text from the action's current binding.</summary>
        public void UpdateBindingDisplay()
        {
            InputAction action = ResolveAction();
            if (action == null)
                return;

            if (_actionNameText != null)
                _actionNameText.text = action.name;

            if (_bindingDisplayText != null)
                _bindingDisplayText.text = action.GetBindingDisplayString(_bindingIndex);
        }

        private InputAction ResolveAction()
        {
            if (_actionReference == null)
            {
                Debug.LogWarning("RebindActionUI has no Action Reference assigned.", this);
                return null;
            }

            return _actionReference.action;
        }

        private void FinishRebind(InputAction action, InputActionRebindingExtensions.RebindingOperation operation)
        {
            operation.Dispose();
            _rebindOperation = null;

            action.Enable();

            if (_waitingForInputIndicator != null)
                _waitingForInputIndicator.SetActive(false);

            UpdateBindingDisplay();
            InputRebindManager.Instance?.SaveBindings();

            OnRebindComplete.Invoke();
        }

        private void CancelRebind(InputAction action, InputActionRebindingExtensions.RebindingOperation operation)
        {
            operation.Dispose();
            _rebindOperation = null;

            action.Enable();

            if (_waitingForInputIndicator != null)
                _waitingForInputIndicator.SetActive(false);

            OnRebindCanceled.Invoke();
        }
    }
}
