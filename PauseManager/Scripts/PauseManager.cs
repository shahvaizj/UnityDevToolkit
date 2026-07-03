using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.PauseManager
{
    /// <summary>
    /// Global pause toggle that drives <see cref="Time.timeScale"/> and notifies listeners via
    /// UnityEvents. Attach to a single persistent GameObject and drive it from anywhere via
    /// <see cref="Instance"/> — no manual wiring between systems required.
    /// <para>
    /// The time scale active when <see cref="Pause"/> is called is remembered and restored on
    /// <see cref="Resume"/>, so slow-motion or speed-up effects are not lost across a pause.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Pause Manager")]
    public class PauseManager : MonoBehaviour
    {
        [Header("Behaviour")]
        [Tooltip("Keep this GameObject alive across scene loads.")]
        [SerializeField] private bool _dontDestroyOnLoad = true;

        [Tooltip("Mute/unmute the AudioListener while paused.")]
        [SerializeField] private bool _muteAudioOnPause = true;

        [Header("Pause Key")]
        [Tooltip("If true, pressing Pause Key toggles pause automatically.")]
        [SerializeField] private bool _enablePauseKey = true;

        [Tooltip("Key that toggles pause when Enable Pause Key is on.")]
        [SerializeField] private KeyCode _pauseKey = KeyCode.Escape;

        [Header("Events")]
        [Tooltip("Fired the moment the game transitions into the paused state.")]
        public UnityEvent OnPause;

        [Tooltip("Fired the moment the game transitions out of the paused state.")]
        public UnityEvent OnResume;

        /// <summary>The active PauseManager instance, if one has been created.</summary>
        public static PauseManager Instance { get; private set; }

        /// <summary>True while the game is currently paused.</summary>
        public bool IsPaused { get; private set; }

        private float _timeScaleBeforePause = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            if (_enablePauseKey && Input.GetKeyDown(_pauseKey))
                TogglePause();
        }

        /// <summary>
        /// Pauses the game by setting <see cref="Time.timeScale"/> to zero. Remembers the
        /// current time scale so it can be restored on <see cref="Resume"/>. No-op if already
        /// paused.
        /// </summary>
        public void Pause()
        {
            if (IsPaused)
                return;

            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            IsPaused = true;

            if (_muteAudioOnPause)
                AudioListener.pause = true;

            OnPause.Invoke();
        }

        /// <summary>
        /// Resumes the game, restoring the time scale that was active before
        /// <see cref="Pause"/> was called. No-op if not currently paused.
        /// </summary>
        public void Resume()
        {
            if (!IsPaused)
                return;

            Time.timeScale = _timeScaleBeforePause;
            IsPaused = false;

            if (_muteAudioOnPause)
                AudioListener.pause = false;

            OnResume.Invoke();
        }

        /// <summary>
        /// Switches between paused and resumed based on the current state.
        /// </summary>
        public void TogglePause()
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }
}
