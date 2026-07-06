using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.ConfirmationDialog
{
    /// <summary>
    /// Reusable modal yes/no confirmation popup. Call <see cref="Show(string, Action{bool})"/>
    /// from anywhere via the static <see cref="Instance"/> to open the dialog with a message
    /// and a callback that receives <c>true</c> if the user confirmed, <c>false</c> if they
    /// cancelled. Wire the Confirm/Cancel buttons in the Inspector and this component handles
    /// showing, hiding, and dispatching the result.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Confirmation Dialog")]
    [RequireComponent(typeof(CanvasGroup))]
    public class ConfirmationDialog : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("TMP label showing the dialog title.")]
        [SerializeField] private TMP_Text _titleLabel;

        [Tooltip("TMP label showing the confirmation message/body text.")]
        [SerializeField] private TMP_Text _messageLabel;

        [Tooltip("Button that confirms the action.")]
        [SerializeField] private Button _confirmButton;

        [Tooltip("Button that cancels the action.")]
        [SerializeField] private Button _cancelButton;

        [Tooltip("Optional TMP label on the confirm button, updated per-call if a custom confirm text is passed.")]
        [SerializeField] private TMP_Text _confirmButtonLabel;

        [Tooltip("Optional TMP label on the cancel button, updated per-call if a custom cancel text is passed.")]
        [SerializeField] private TMP_Text _cancelButtonLabel;

        [Header("Default Text")]
        [SerializeField] private string _defaultTitle = "Are you sure?";
        [SerializeField] private string _defaultConfirmText = "Yes";
        [SerializeField] private string _defaultCancelText = "No";

        [Header("Behaviour")]
        [Tooltip("Hide the dialog on Awake. Disable if you want it visible in the editor for layout purposes only.")]
        [SerializeField] private bool _hiddenOnAwake = true;

        [Header("Events")]
        [Tooltip("Fired when the dialog opens.")]
        public UnityEvent OnOpened;

        [Tooltip("Fired when the dialog closes, for any reason (confirm, cancel, or Close()).")]
        public UnityEvent OnClosed;

        [Tooltip("Fired when the user clicks Confirm.")]
        public UnityEvent OnConfirmed;

        [Tooltip("Fired when the user clicks Cancel.")]
        public UnityEvent OnCancelled;

        /// <summary>Most recently created instance, for convenient global access.</summary>
        public static ConfirmationDialog Instance { get; private set; }

        /// <summary>True while the dialog is currently visible and awaiting a response.</summary>
        public bool IsOpen { get; private set; }

        private CanvasGroup _canvasGroup;
        private Action<bool> _onResult;

        private void Awake()
        {
            Instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_confirmButton != null)
                _confirmButton.onClick.AddListener(Confirm);

            if (_cancelButton != null)
                _cancelButton.onClick.AddListener(Cancel);

            if (_hiddenOnAwake)
                SetVisible(false);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Opens the dialog with the default title and the given message.
        /// </summary>
        /// <param name="message">Body text describing what is being confirmed.</param>
        /// <param name="onResult">Invoked with <c>true</c> on confirm, <c>false</c> on cancel.</param>
        public void Show(string message, Action<bool> onResult = null)
        {
            Show(_defaultTitle, message, onResult);
        }

        /// <summary>
        /// Opens the dialog with a custom title, message, and optional per-call button labels.
        /// </summary>
        /// <param name="title">Dialog title text.</param>
        /// <param name="message">Body text describing what is being confirmed.</param>
        /// <param name="onResult">Invoked with <c>true</c> on confirm, <c>false</c> on cancel.</param>
        /// <param name="confirmText">Overrides the confirm button label for this call only. Pass null to keep the default.</param>
        /// <param name="cancelText">Overrides the cancel button label for this call only. Pass null to keep the default.</param>
        public void Show(string title, string message, Action<bool> onResult = null, string confirmText = null, string cancelText = null)
        {
            _onResult = onResult;

            if (_titleLabel != null)
                _titleLabel.text = title;

            if (_messageLabel != null)
                _messageLabel.text = message;

            if (_confirmButtonLabel != null)
                _confirmButtonLabel.text = string.IsNullOrEmpty(confirmText) ? _defaultConfirmText : confirmText;

            if (_cancelButtonLabel != null)
                _cancelButtonLabel.text = string.IsNullOrEmpty(cancelText) ? _defaultCancelText : cancelText;

            SetVisible(true);
            IsOpen = true;
            OnOpened.Invoke();
        }

        /// <summary>
        /// Confirms the dialog: closes it, then invokes the result callback and events.
        /// Wired to the confirm button's <c>onClick</c> automatically; call directly for
        /// keyboard/gamepad-driven confirmation.
        /// </summary>
        public void Confirm()
        {
            if (!IsOpen)
                return;

            Action<bool> callback = _onResult;
            CloseInternal();

            callback?.Invoke(true);
            OnConfirmed.Invoke();
        }

        /// <summary>
        /// Cancels the dialog: closes it, then invokes the result callback and events.
        /// Wired to the cancel button's <c>onClick</c> automatically; call directly for
        /// keyboard/gamepad-driven cancellation.
        /// </summary>
        public void Cancel()
        {
            if (!IsOpen)
                return;

            Action<bool> callback = _onResult;
            CloseInternal();

            callback?.Invoke(false);
            OnCancelled.Invoke();
        }

        /// <summary>
        /// Closes the dialog without invoking the result callback or the Confirmed/Cancelled
        /// events. Useful for dismissing the popup programmatically (e.g. the underlying
        /// action became unavailable). <see cref="OnClosed"/> still fires.
        /// </summary>
        public void Close()
        {
            if (!IsOpen)
                return;

            CloseInternal();
        }

        // ─── Internal ─────────────────────────────────────────────────────────

        private void CloseInternal()
        {
            _onResult = null;
            SetVisible(false);
            IsOpen = false;
            OnClosed.Invoke();
        }

        private void SetVisible(bool visible)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }
    }
}
