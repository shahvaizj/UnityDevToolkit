using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ShahvaizJ.InputRebindUI
{
    /// <summary>
    /// Persists binding overrides for an <see cref="InputActionAsset"/> to <see cref="PlayerPrefs"/>
    /// so players keep their rebinds between sessions. Place one instance in a persistent scene and
    /// point it at the same asset used by your <see cref="RebindActionUI"/> rows — they look up
    /// <see cref="Instance"/> automatically and save through it after each successful rebind.
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Input Rebind UI/Input Rebind Manager")]
    public class InputRebindManager : MonoBehaviour
    {
        [Tooltip("The action asset whose binding overrides are saved/loaded. Must match the asset used by your RebindActionUI rows.")]
        [SerializeField] private InputActionAsset _actionAsset;

        [Tooltip("PlayerPrefs key the binding overrides JSON is stored under.")]
        [SerializeField] private string _playerPrefsKey = "InputRebindUI.Overrides";

        [Tooltip("Load saved overrides automatically on Awake.")]
        [SerializeField] private bool _loadOnAwake = true;

        [Header("Events")]
        [Tooltip("Fired after binding overrides are written to PlayerPrefs.")]
        public UnityEvent OnBindingsSaved;

        [Tooltip("Fired after saved binding overrides are applied to the action asset.")]
        public UnityEvent OnBindingsLoaded;

        [Tooltip("Fired after all binding overrides are cleared.")]
        public UnityEvent OnBindingsReset;

        /// <summary>The active instance, used by <see cref="RebindActionUI"/> rows to auto-save. Null if none exists in the scene.</summary>
        public static InputRebindManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            if (_loadOnAwake)
                LoadBindings();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Serializes all current binding overrides on the assigned asset to JSON and writes them
        /// to <see cref="PlayerPrefs"/>.
        /// </summary>
        public void SaveBindings()
        {
            if (_actionAsset == null)
            {
                Debug.LogWarning("InputRebindManager has no Action Asset assigned — nothing to save.", this);
                return;
            }

            string json = _actionAsset.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(_playerPrefsKey, json);
            PlayerPrefs.Save();

            OnBindingsSaved.Invoke();
        }

        /// <summary>
        /// Reads previously saved binding overrides from <see cref="PlayerPrefs"/> and applies them
        /// to the assigned asset. Does nothing if no overrides have been saved yet.
        /// </summary>
        public void LoadBindings()
        {
            if (_actionAsset == null)
            {
                Debug.LogWarning("InputRebindManager has no Action Asset assigned — nothing to load.", this);
                return;
            }

            if (!PlayerPrefs.HasKey(_playerPrefsKey))
                return;

            string json = PlayerPrefs.GetString(_playerPrefsKey);
            _actionAsset.LoadBindingOverridesFromJson(json);

            OnBindingsLoaded.Invoke();
        }

        /// <summary>
        /// Clears all binding overrides on the assigned asset, reverting every action to its
        /// default bindings, and removes the saved entry from <see cref="PlayerPrefs"/>.
        /// </summary>
        public void ResetAllBindings()
        {
            if (_actionAsset == null)
            {
                Debug.LogWarning("InputRebindManager has no Action Asset assigned — nothing to reset.", this);
                return;
            }

            _actionAsset.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey(_playerPrefsKey);
            PlayerPrefs.Save();

            OnBindingsReset.Invoke();
        }
    }
}
