using UnityEngine;

namespace ShahvaizJ.SingletonBase
{
    /// <summary>
    /// Generic MonoBehaviour singleton base class. Inherit from this to make any
    /// MonoBehaviour a singleton with automatic instance management, optional
    /// scene persistence, and configurable duplicate handling.
    /// <para>
    /// Usage: <c>public class GameManager : Singleton&lt;GameManager&gt; { }</c>
    /// </para>
    /// </summary>
    /// <typeparam name="T">The concrete MonoBehaviour type.</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [Header("Singleton Settings")]
        [Tooltip("Keep this instance alive across scene loads via DontDestroyOnLoad.")]
        [SerializeField] private bool _persistAcrossScenes = true;

        [Tooltip("What to do when a second instance of this singleton is detected.")]
        [SerializeField] private DuplicatePolicy _duplicatePolicy = DuplicatePolicy.DestroyNewest;

        private static T _instance;
        private static bool _applicationIsQuitting;

        /// <summary>
        /// The singleton instance. Returns <c>null</c> if no instance exists or the
        /// application is shutting down.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning(
                        $"[Singleton] {typeof(T).Name} instance requested after application quit — returning null.");
                    return null;
                }

                return _instance;
            }
        }

        /// <summary>True if a singleton instance currently exists and the application is running.</summary>
        public static bool HasInstance => _instance != null && !_applicationIsQuitting;

        /// <summary>Whether this instance persists across scene loads.</summary>
        public bool PersistAcrossScenes => _persistAcrossScenes;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                HandleDuplicate();
                return;
            }

            RegisterInstance();
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                OnBeforeDestroyed();
                _instance = null;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        /// <summary>
        /// Called once after this instance becomes the active singleton.
        /// Override this instead of <c>Awake</c> for subclass initialization.
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// Called just before the singleton instance is destroyed and the static
        /// reference is cleared. Override this for cleanup logic.
        /// </summary>
        protected virtual void OnBeforeDestroyed() { }

        private void RegisterInstance()
        {
            _instance = this as T;
            _applicationIsQuitting = false;

            if (_persistAcrossScenes)
            {
                if (transform.parent != null)
                    transform.SetParent(null);

                DontDestroyOnLoad(gameObject);
            }

            OnInitialized();
        }

        private void HandleDuplicate()
        {
            switch (_duplicatePolicy)
            {
                case DuplicatePolicy.DestroyNewest:
                    Debug.LogWarning(
                        $"[Singleton] Duplicate {typeof(T).Name} on \"{gameObject.name}\" — " +
                        "destroying the new instance.", this);
                    Destroy(gameObject);
                    break;

                case DuplicatePolicy.DestroyOldest:
                    Debug.LogWarning(
                        $"[Singleton] Duplicate {typeof(T).Name} — destroying the old instance " +
                        $"on \"{_instance.gameObject.name}\".", _instance);
                    Destroy(_instance.gameObject);
                    RegisterInstance();
                    break;
            }
        }
    }

    /// <summary>Determines what happens when a second singleton instance appears in the scene.</summary>
    public enum DuplicatePolicy
    {
        /// <summary>Keep the original instance and destroy the new one.</summary>
        DestroyNewest,

        /// <summary>Destroy the original instance and replace it with the new one.</summary>
        DestroyOldest
    }
}
