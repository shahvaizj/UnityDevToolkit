using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.AdvancedToggle
{
    /// <summary>
    /// A flexible, generic toggle component with optional usage limits,
    /// visual color feedback for both UI and 3D contexts, and rich
    /// UnityEvent callbacks. Zero external dependencies beyond Unity itself.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Advanced Toggle")]
    public class AdvancedToggle : MonoBehaviour
    {
        // -------------------------------------------------------------------------
        // Types
        // -------------------------------------------------------------------------

        /// <summary>Controls how many times each toggle direction can be used.</summary>
        public enum ToggleMode
        {
            /// <summary>Toggle can be flipped an unlimited number of times.</summary>
            NormalToggle,
            /// <summary>Both On and Off directions are limited independently.</summary>
            OnOffLimited,
            /// <summary>Only the On direction is limited.</summary>
            OnLimited,
            /// <summary>Only the Off direction is limited.</summary>
            OffLimited
        }

        // -------------------------------------------------------------------------
        // Inspector fields
        // -------------------------------------------------------------------------

        [Header("Initial State")]
        [Tooltip("Starting state of the toggle.")]
        [SerializeField] private bool isOn = false;
        [Tooltip("When enabled, SetState is called on Start to apply the initial state " +
                 "and fire visual feedback without counting against usage limits.")]
        [SerializeField] private bool applyStateOnStart = false;

        [Header("Toggle Limits")]
        [SerializeField] private ToggleMode mode = ToggleMode.NormalToggle;
        [Tooltip("Maximum number of times the toggle can be turned On.")]
        [SerializeField] private int maxOnCount = 1;
        [Tooltip("Maximum number of times the toggle can be turned Off.")]
        [SerializeField] private int maxOffCount = 1;

        [Header("Visual Feedback (Optional)")]
        [Tooltip("UI Graphic (Image, Text, etc.) whose color changes on toggle. Leave empty to skip.")]
        [SerializeField] private Graphic targetGraphic;
        [Tooltip("3D/2D Renderer (MeshRenderer, SpriteRenderer, etc.) whose material color " +
                 "changes on toggle. Leave empty to skip.")]
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Color onColor = Color.green;
        [SerializeField] private Color offColor = Color.red;

        [Header("Events")]
        [Tooltip("Fired every time the toggle turns on.")]
        public UnityEvent OnToggledOn;
        [Tooltip("Fired every time the toggle turns off.")]
        public UnityEvent OnToggledOff;
        [Tooltip("Fired when the On-direction limit is reached.")]
        public UnityEvent OnLimitReached;
        [Tooltip("Fired when the Off-direction limit is reached.")]
        public UnityEvent OffLimitReached;
        [Tooltip("Fired once when all applicable limits are exhausted. " +
                 "Wire any disabling logic here (e.g. deactivate a Selectable or Button).")]
        public UnityEvent OnInteractionDisabled;

        [Header("Debug Counters")]
        [SerializeField] private int onEventCount = 0;
        [SerializeField] private int offEventCount = 0;

        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>Current on/off state of the toggle.</summary>
        public bool IsOn => isOn;

        /// <summary>Number of times the toggle has been turned on.</summary>
        public int OnEventCount => onEventCount;

        /// <summary>Number of times the toggle has been turned off.</summary>
        public int OffEventCount => offEventCount;

        // -------------------------------------------------------------------------
        // Unity lifecycle
        // -------------------------------------------------------------------------

        private void Start()
        {
            if (applyStateOnStart)
                SetState(isOn);
        }

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>Flips the toggle to the opposite state.</summary>
        public void Toggle() => SetState(!isOn);

        /// <summary>Forces the toggle into the specified state.</summary>
        /// <param name="newState"><c>true</c> = On, <c>false</c> = Off.</param>
        public void SetState(bool newState)
        {
            // Cache flag before any mutation so helper methods can read it correctly.
            bool isInitialApply = applyStateOnStart;

            if (isOn == newState && !isInitialApply) return;

            // Block state change if the relevant limit is already exhausted.
            if (!isInitialApply)
            {
                if (newState  && IsOnLimited()  && onEventCount  >= maxOnCount)  return;
                if (!newState && IsOffLimited() && offEventCount >= maxOffCount) return;
            }

            isOn = newState;
            applyStateOnStart = false; // clear early so helpers don't re-read it

            ApplyVisualFeedback(isOn);

            if (isOn)
            {
                OnToggledOn?.Invoke();
                if (!isInitialApply) HandleOnLimit();
            }
            else
            {
                OnToggledOff?.Invoke();
                if (!isInitialApply) HandleOffLimit();
            }
        }

        /// <summary>Resets the usage counters, re-enabling a previously exhausted limited toggle.</summary>
        public void ResetCounters()
        {
            onEventCount = 0;
            offEventCount = 0;
        }

        /// <summary>
        /// Overrides the maximum number of times each direction can be toggled at runtime.
        /// Pass <c>-1</c> to make a direction unlimited (note: the ToggleMode must also permit it).
        /// </summary>
        /// <param name="maxOn">Maximum On-toggles.</param>
        /// <param name="maxOff">Maximum Off-toggles.</param>
        public void SetLimits(int maxOn, int maxOff)
        {
            maxOnCount  = maxOn;
            maxOffCount = maxOff;
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        private bool IsOnLimited()  => mode == ToggleMode.OnLimited  || mode == ToggleMode.OnOffLimited;
        private bool IsOffLimited() => mode == ToggleMode.OffLimited || mode == ToggleMode.OnOffLimited;

        private void ApplyVisualFeedback(bool state)
        {
            Color color = state ? onColor : offColor;

            if (targetGraphic != null)
                targetGraphic.color = color;

            if (targetRenderer != null)
                targetRenderer.material.color = color;
        }

        private void HandleOnLimit()
        {
            if (!IsOnLimited()) return;

            onEventCount++;
            if (onEventCount >= maxOnCount)
            {
                OnLimitReached?.Invoke();
                TryFireInteractionDisabled();
            }
        }

        private void HandleOffLimit()
        {
            if (!IsOffLimited()) return;

            offEventCount++;
            if (offEventCount >= maxOffCount)
            {
                OffLimitReached?.Invoke();
                TryFireInteractionDisabled();
            }
        }

        /// <summary>
        /// Fires <see cref="OnInteractionDisabled"/> only when every applicable limit
        /// has been exhausted (so OnOffLimited waits for both counters).
        /// </summary>
        private void TryFireInteractionDisabled()
        {
            if (mode == ToggleMode.NormalToggle) return;

            bool onExhausted  = !IsOnLimited()  || onEventCount  >= maxOnCount;
            bool offExhausted = !IsOffLimited() || offEventCount >= maxOffCount;

            if (onExhausted && offExhausted)
                OnInteractionDisabled?.Invoke();
        }
    }
}
