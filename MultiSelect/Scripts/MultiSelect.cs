using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace ShahvaizJ.MultiSelect
{
    /// <summary>
    /// A horizontal option selector: left and right arrow buttons cycle through a list of
    /// string options, with the current value shown on a label. Populate <see cref="_options"/>
    /// in the Inspector (or at runtime via <see cref="SetOptions"/>). On <c>Awake</c> the
    /// list is applied to the label; read the chosen index from <see cref="CurSelected"/>
    /// and its text from <see cref="Value"/>.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Multi Select")]
    public class MultiSelect : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        [Header("Label")]
        [SerializeField] private TMP_Text _label;

        [Header("Options")]
        [SerializeField] private string[] _options;
        [Tooltip("Index selected on Awake. Clamped to the available options.")]
        [SerializeField] private int _curSelected = 0;
        [Tooltip("When enabled, paging past either end wraps around to the other side.")]
        [SerializeField] private bool _wrap = true;

        [Header("Events")]
        [Tooltip("Fired with the new index whenever the selection changes.")]
        public UnityEvent<int> OnSelectionChanged;

        [Tooltip("Fired with the new option text whenever the selection changes.")]
        public UnityEvent<string> OnValueChanged;

        /// <summary>Index of the currently selected option.</summary>
        public int CurSelected => _curSelected;

        /// <summary>The selected option's text, or an empty string when there are no options.</summary>
        public string Value => HasOptions ? _options[_curSelected] : string.Empty;

        /// <summary>Number of available options.</summary>
        public int Count => _options != null ? _options.Length : 0;

        /// <summary>True when at least one option is available.</summary>
        public bool HasOptions => _options != null && _options.Length > 0;

        private void Awake()
        {
            if (_leftButton != null)
                _leftButton.onClick.AddListener(Previous);
            if (_rightButton != null)
                _rightButton.onClick.AddListener(Next);

            ClampSelection();
            Refresh();
        }

        private void OnDestroy()
        {
            if (_leftButton != null)
                _leftButton.onClick.RemoveListener(Previous);
            if (_rightButton != null)
                _rightButton.onClick.RemoveListener(Next);
        }

        /// <summary>Advances to the next option (right arrow).</summary>
        public void Next() => Step(1);

        /// <summary>Returns to the previous option (left arrow).</summary>
        public void Previous() => Step(-1);

        private void Step(int direction)
        {
            if (!HasOptions)
                return;

            int next = _curSelected + direction;

            if (_wrap)
                next = (next % Count + Count) % Count;
            else
                next = Mathf.Clamp(next, 0, Count - 1);

            SetSelected(next);
        }

        /// <summary>
        /// Selects the option at <paramref name="index"/>, refreshes the label, and fires the
        /// change events. Out-of-range indices are ignored.
        /// </summary>
        /// <param name="index">The option index to select.</param>
        public void SetSelected(int index)
        {
            if (!HasOptions || index < 0 || index >= Count)
                return;

            bool changed = index != _curSelected;
            _curSelected = index;
            Refresh();

            if (changed)
            {
                OnSelectionChanged.Invoke(_curSelected);
                OnValueChanged.Invoke(Value);
            }
        }

        /// <summary>
        /// Selects the first option whose text equals <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The option text to look for.</param>
        /// <returns>True if a matching option was found and selected.</returns>
        public bool SetValue(string value)
        {
            if (!HasOptions)
                return false;

            for (int i = 0; i < _options.Length; i++)
            {
                if (_options[i] == value)
                {
                    SetSelected(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replaces the option list at runtime and resets the selection to the first entry.
        /// </summary>
        /// <param name="options">The new set of options.</param>
        public void SetOptions(string[] options)
        {
            _options = options;
            _curSelected = 0;
            ClampSelection();
            Refresh();
            OnSelectionChanged.Invoke(_curSelected);
            OnValueChanged.Invoke(Value);
        }

        private void ClampSelection()
        {
            _curSelected = HasOptions ? Mathf.Clamp(_curSelected, 0, Count - 1) : 0;
        }

        private void Refresh()
        {
            if (_label != null)
                _label.text = Value;

            // With wrapping off, disable the arrows at the list's edges.
            if (!_wrap)
            {
                if (_leftButton != null)
                    _leftButton.interactable = HasOptions && _curSelected > 0;
                if (_rightButton != null)
                    _rightButton.interactable = HasOptions && _curSelected < Count - 1;
            }
        }
    }
}
