using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.TimerUtil
{
    public class TimerController : MonoBehaviour
    {
        [Header("Settings")]
        [Min(0.1f)] public float duration = 10f;
        public bool countDown = true;
        public bool autoStart = false;
        public bool loop = false;

        [Header("Audio - One Shots")]
        public AudioSource audioSource;
        public AudioClip startSound;
        public AudioClip endSound;
        public AudioClip tickSound;
        [Tooltip("Tick sound fires at every N percent. E.g. 10 = plays at 10, 20, 30...")]
        [Range(1, 100)] public int tickPercentageInterval = 10;

        [Header("Audio - Loop")]
        public AudioSource loopBGSource;
        public AudioClip loopBGSound;
        [Range(0f, 1f)] public float loopBGVolume = 1f;

        [Header("Audio - Last Warning")]
        public AudioSource lastWarningSource;
        public AudioClip lastWarningSound;
        [Range(0f, 1f)] public float lastWarningVolume = 1f;
        [Tooltip("How many seconds before the end the warning loop starts")]
        [Min(0f)] public float lastWarningSeconds = 5f;

        [Header("UI")]
        public TimerUI timerUI;

        [Header("Events")]
        public UnityEvent onTimerStarted;
        public UnityEvent onTimerCompleted;
        public UnityEvent onTimerStopped;
        public UnityEvent onTimerPaused;
        public UnityEvent onTimerResumed;

        [Header("Runtime")]
        [SerializeField] private float currentTime = 0f;
        [SerializeField] private bool isRunning = false;
        [SerializeField] private bool isPaused = false;

        private int lastTickMilestone = 0;
        private bool lastWarningTriggered = false;

        public float CurrentTime => currentTime;
        public float NormalizedProgress => duration > 0f ? currentTime / duration : 0f;
        public bool IsRunning => isRunning;
        public bool IsPaused => isPaused;

        private void Start()
        {
            if (autoStart)
                StartTimer();
        }

        private void Update()
        {
            if (!isRunning || isPaused) return;

            currentTime += Time.deltaTime;

            if (currentTime >= duration)
            {
                currentTime = duration;
                timerUI?.Tick(currentTime, duration, countDown);
                Complete();
                return;
            }

            timerUI?.Tick(currentTime, duration, countDown);
            CheckTickMilestone();
            CheckLastWarning();
        }

        public void StartTimer()
        {
            currentTime = 0f;
            lastTickMilestone = 0;
            lastWarningTriggered = false;
            isRunning = true;
            isPaused = false;
            timerUI?.Show();
            timerUI?.Tick(currentTime, duration, countDown);
            PlayLoopBG();
            PlaySound(startSound);
            onTimerStarted?.Invoke();
        }

        public void StopTimer()
        {
            isRunning = false;
            isPaused = false;
            StopLoopSounds();
            timerUI?.Hide();
            onTimerStopped?.Invoke();
        }

        public void PauseTimer()
        {
            if (!isRunning || isPaused) return;
            isPaused = true;
            onTimerPaused?.Invoke();
        }

        public void ResumeTimer()
        {
            if (!isRunning || !isPaused) return;
            isPaused = false;
            onTimerResumed?.Invoke();
        }

        public void ResetTimer()
        {
            currentTime = 0f;
            isRunning = false;
            isPaused = false;
        }

        public void SetDuration(float newDuration)
        {
            duration = Mathf.Max(0.1f, newDuration);
        }

        private void Complete()
        {
            isRunning = false;
            isPaused = false;
            StopLoopSounds();
            PlaySound(endSound);
            timerUI?.Hide();
            onTimerCompleted?.Invoke();

            if (loop)
                StartTimer();
        }

        private void CheckTickMilestone()
        {
            if (tickSound == null || tickPercentageInterval <= 0) return;

            int currentPercent = Mathf.FloorToInt(NormalizedProgress * 100f);
            int nextMilestone = lastTickMilestone + tickPercentageInterval;

            if (currentPercent >= nextMilestone)
            {
                lastTickMilestone = (currentPercent / tickPercentageInterval) * tickPercentageInterval;
                PlaySound(tickSound);
            }
        }

        private void CheckLastWarning()
        {
            if (lastWarningTriggered || lastWarningSeconds <= 0f || lastWarningSound == null) return;
            if (duration - currentTime <= lastWarningSeconds)
            {
                lastWarningTriggered = true;
                StopLoopBG();
                PlayLastWarning();
            }
        }

        private void PlayLoopBG()
        {
            if (loopBGSource == null || loopBGSound == null) return;
            loopBGSource.clip = loopBGSound;
            loopBGSource.volume = loopBGVolume;
            loopBGSource.loop = true;
            loopBGSource.Play();
        }

        private void StopLoopBG()
        {
            if (loopBGSource != null && loopBGSource.isPlaying)
                loopBGSource.Stop();
        }

        private void PlayLastWarning()
        {
            if (lastWarningSource == null || lastWarningSound == null) return;
            lastWarningSource.clip = lastWarningSound;
            lastWarningSource.volume = lastWarningVolume;
            lastWarningSource.loop = true;
            lastWarningSource.Play();
        }

        private void StopLastWarning()
        {
            if (lastWarningSource != null && lastWarningSource.isPlaying)
                lastWarningSource.Stop();
        }

        private void StopLoopSounds()
        {
            StopLoopBG();
            StopLastWarning();
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null || audioSource == null) return;
            audioSource.PlayOneShot(clip);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(TimerController))]
    public class TimerControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TimerController timer = (TimerController)target;

            if (!Application.isPlaying) return;

            UnityEditor.EditorGUILayout.Space(6);
            UnityEditor.EditorGUILayout.LabelField("Runtime", UnityEditor.EditorStyles.boldLabel);

            GUI.enabled = false;
            UnityEditor.EditorGUILayout.FloatField("Elapsed", timer.CurrentTime);
            UnityEditor.EditorGUILayout.Slider("Progress", timer.NormalizedProgress, 0f, 1f);
            GUI.enabled = true;

            UnityEditor.EditorGUILayout.Space(4);
            UnityEditor.EditorGUILayout.LabelField("Controls", UnityEditor.EditorStyles.boldLabel);

            UnityEditor.EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))  timer.StartTimer();
            if (GUILayout.Button("Stop"))   timer.StopTimer();
            if (GUILayout.Button("Reset"))  timer.ResetTimer();
            UnityEditor.EditorGUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause"))  timer.PauseTimer();
            if (GUILayout.Button("Resume")) timer.ResumeTimer();
            UnityEditor.EditorGUILayout.EndHorizontal();

            Repaint();
        }
    }
#endif
}
