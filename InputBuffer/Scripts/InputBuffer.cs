using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ShahvaizJ.InputBuffer
{
    /// <summary>
    /// One action tracked by an <see cref="InputBuffer"/>, with an optional per-action buffer
    /// window override.
    /// </summary>
    [Serializable]
    public class BufferedAction
    {
        [Tooltip("The input action to buffer presses for.")]
        public InputActionReference ActionReference;

        [Tooltip("Buffer window in seconds for this action. Set to a negative value to fall back to the InputBuffer's Default Buffer Window.")]
        public float BufferWindowOverride = -1f;
    }

    /// <summary>
    /// Remembers recent action presses for a short window so a query made a few frames after the
    /// physical press (e.g. "can I combo now?", "did the player press Jump just before landing?")
    /// still sees it. Common in fighting/action games so inputs pressed slightly early aren't
    /// dropped on the floor. Assign actions in the Inspector or call <see cref="Register"/> at
    /// runtime, then poll <see cref="WasPressed(InputActionReference)"/> or
    /// <see cref="TryConsume(InputActionReference)"/> from your gameplay code.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Input Buffer/Input Buffer")]
    public class InputBuffer : MonoBehaviour
    {
        [Tooltip("Default buffer window in seconds for actions that don't specify an override. 0.15s covers roughly 9 frames at 60fps.")]
        [SerializeField] private float _defaultBufferWindow = 0.15f;

        [Tooltip("Enable each registered action on OnEnable and disable it on OnDisable. Turn off if another system (e.g. PlayerInput) already manages enabling these actions.")]
        [SerializeField] private bool _autoEnableActions = true;

        [Tooltip("Actions this buffer tracks presses for.")]
        [SerializeField] private List<BufferedAction> _bufferedActions = new List<BufferedAction>();

        [Header("Events")]
        [Tooltip("Fired with the action's name whenever a buffered press is consumed via TryConsume.")]
        public UnityEvent<string> OnBufferedInputConsumed;

        private readonly Dictionary<InputAction, float> _pressTimestamps = new Dictionary<InputAction, float>();
        private readonly Dictionary<InputAction, float> _bufferWindows = new Dictionary<InputAction, float>();
        private readonly HashSet<InputAction> _subscribedActions = new HashSet<InputAction>();

        private void OnEnable()
        {
            foreach (BufferedAction entry in _bufferedActions)
                RegisterEntry(entry);
        }

        private void OnDisable()
        {
            foreach (InputAction action in _subscribedActions)
            {
                action.performed -= OnActionPerformed;

                if (_autoEnableActions)
                    action.Disable();
            }

            _subscribedActions.Clear();
        }

        private void RegisterEntry(BufferedAction entry)
        {
            InputAction action = entry?.ActionReference != null ? entry.ActionReference.action : null;
            if (action == null)
                return;

            _bufferWindows[action] = entry.BufferWindowOverride >= 0f ? entry.BufferWindowOverride : _defaultBufferWindow;

            if (_subscribedActions.Add(action))
                action.performed += OnActionPerformed;

            if (_autoEnableActions)
                action.Enable();
        }

        private void OnActionPerformed(InputAction.CallbackContext context)
        {
            _pressTimestamps[context.action] = Time.unscaledTime;
        }

        /// <summary>
        /// Registers an action for buffering at runtime, in addition to any configured in the Inspector.
        /// </summary>
        /// <param name="actionReference">The action to buffer presses for.</param>
        /// <param name="bufferWindowOverride">Buffer window in seconds, or a negative value to use the Default Buffer Window.</param>
        public void Register(InputActionReference actionReference, float bufferWindowOverride = -1f)
        {
            if (actionReference == null)
                return;

            var entry = new BufferedAction { ActionReference = actionReference, BufferWindowOverride = bufferWindowOverride };
            _bufferedActions.Add(entry);
            RegisterEntry(entry);
        }

        /// <summary>
        /// Stops buffering the given action and forgets any press currently buffered for it.
        /// </summary>
        public void Unregister(InputActionReference actionReference)
        {
            InputAction action = actionReference != null ? actionReference.action : null;
            if (action == null)
                return;

            if (_subscribedActions.Remove(action))
            {
                action.performed -= OnActionPerformed;

                if (_autoEnableActions)
                    action.Disable();
            }

            _pressTimestamps.Remove(action);
            _bufferWindows.Remove(action);
            _bufferedActions.RemoveAll(entry => entry.ActionReference == actionReference);
        }

        /// <summary>Returns true if the action was pressed within its buffer window and hasn't been consumed yet.</summary>
        public bool WasPressed(InputActionReference actionReference)
        {
            return WasPressed(actionReference != null ? actionReference.action : null);
        }

        /// <summary>Returns true if the action was pressed within its buffer window and hasn't been consumed yet.</summary>
        public bool WasPressed(InputAction action)
        {
            if (action == null || !_pressTimestamps.TryGetValue(action, out float timestamp))
                return false;

            float window = _bufferWindows.TryGetValue(action, out float bufferWindow) ? bufferWindow : _defaultBufferWindow;
            return Time.unscaledTime - timestamp <= window;
        }

        /// <summary>
        /// If the action was pressed within its buffer window, consumes the buffered press (so it won't
        /// be returned again until the next physical press) and returns true. Use this in gameplay code
        /// that should react to a buffered input exactly once, such as triggering a combo attack.
        /// </summary>
        public bool TryConsume(InputActionReference actionReference)
        {
            return TryConsume(actionReference != null ? actionReference.action : null);
        }

        /// <summary>
        /// If the action was pressed within its buffer window, consumes the buffered press (so it won't
        /// be returned again until the next physical press) and returns true. Use this in gameplay code
        /// that should react to a buffered input exactly once, such as triggering a combo attack.
        /// </summary>
        public bool TryConsume(InputAction action)
        {
            if (!WasPressed(action))
                return false;

            _pressTimestamps.Remove(action);
            OnBufferedInputConsumed?.Invoke(action.name);
            return true;
        }

        /// <summary>Seconds remaining before a buffered press for this action expires, or 0 if none is buffered.</summary>
        public float GetBufferedTimeRemaining(InputActionReference actionReference)
        {
            return GetBufferedTimeRemaining(actionReference != null ? actionReference.action : null);
        }

        /// <summary>Seconds remaining before a buffered press for this action expires, or 0 if none is buffered.</summary>
        public float GetBufferedTimeRemaining(InputAction action)
        {
            if (action == null || !_pressTimestamps.TryGetValue(action, out float timestamp))
                return 0f;

            float window = _bufferWindows.TryGetValue(action, out float bufferWindow) ? bufferWindow : _defaultBufferWindow;
            return Mathf.Max(0f, window - (Time.unscaledTime - timestamp));
        }

        /// <summary>Forgets every currently buffered press without firing any events. Useful on state transitions (e.g. entering a cutscene) where stale input shouldn't carry over.</summary>
        public void ClearAll()
        {
            _pressTimestamps.Clear();
        }
    }
}
