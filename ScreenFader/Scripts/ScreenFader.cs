using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShahvaizJ.ScreenFader
{
    /// <summary>
    /// Full-screen fade overlay for scene transitions, cutscenes, and death sequences.
    /// <para>
    /// <see cref="FadeOut"/> transitions the screen to the configured fade color (e.g. black).
    /// <see cref="FadeIn"/> transitions back from that color to transparent.
    /// Both accept an optional <see cref="Action"/> callback fired on completion, making
    /// them easy to chain into coroutine or async-driven transition pipelines.
    /// </para>
    /// <para>
    /// Attach this component to a Canvas GameObject with a full-screen child Image and
    /// access it globally via <see cref="Instance"/> from any script.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Screen Fader")]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ScreenFader : MonoBehaviour
    {
        [Header("Overlay")]
        [Tooltip("A full-screen UI Image that covers the entire Canvas. Set its color to the desired fade color; alpha is ignored — CanvasGroup drives it.")]
        [SerializeField] private Image _overlay;

        [Tooltip("Color the screen fades to during FadeOut. White, black, or any custom color.")]
        [SerializeField] private Color _fadeColor = Color.black;

        [Header("Timing")]
        [Tooltip("Fade duration in seconds used when no explicit duration is passed to FadeIn/FadeOut.")]
        [SerializeField] private float _defaultDuration = 0.5f;

        [Tooltip("Easing curve: X is normalized time (0–1), Y is normalized alpha (0–1). " +
                 "Maps the fade to opaque direction; FadeIn uses the same curve in reverse.")]
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("When true, fades tick on unscaled time so they animate even while Time.timeScale = 0.")]
        [SerializeField] private bool _useUnscaledTime = true;

        [Header("Events")]
        [Tooltip("Fired when a FadeOut completes and the overlay is fully opaque.")]
        public UnityEvent OnFadeOutComplete;

        [Tooltip("Fired when a FadeIn completes and the overlay is fully transparent.")]
        public UnityEvent OnFadeInComplete;

        /// <summary>The most recently registered instance for convenient global access.</summary>
        public static ScreenFader Instance { get; private set; }

        /// <summary>True while a fade animation is actively running.</summary>
        public bool IsFading { get; private set; }

        /// <summary>True when the overlay is at full opacity (screen is blacked/colored out).</summary>
        public bool IsOpaque => _canvasGroup != null && _canvasGroup.alpha >= 1f;

        /// <summary>True when the overlay is fully transparent (scene is fully visible).</summary>
        public bool IsTransparent => _canvasGroup != null && _canvasGroup.alpha <= 0f;

        /// <summary>Current overlay alpha (0 = transparent, 1 = fully covering the screen).</summary>
        public float Alpha => _canvasGroup != null ? _canvasGroup.alpha : 0f;

        private CanvasGroup _canvasGroup;
        private Coroutine _activeCoroutine;

        private void Awake()
        {
            Instance = this;

            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;

            Canvas canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;

            if (_overlay != null)
            {
                Color c = _fadeColor;
                c.a = 1f;
                _overlay.color = c;
                _overlay.raycastTarget = false;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Fade the screen out (transparent → fade color) using the Inspector's default duration.
        /// </summary>
        /// <param name="onComplete">Optional callback invoked when the overlay reaches full opacity.</param>
        public void FadeOut(Action onComplete = null)
        {
            FadeOut(_defaultDuration, onComplete);
        }

        /// <summary>
        /// Fade the screen out (transparent → fade color) over <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="duration">Fade duration in seconds.</param>
        /// <param name="onComplete">Optional callback invoked when the overlay reaches full opacity.</param>
        public void FadeOut(float duration, Action onComplete = null)
        {
            BeginFade(0f, 1f, duration, () =>
            {
                _canvasGroup.blocksRaycasts = true;
                OnFadeOutComplete.Invoke();
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Fade the screen in (fade color → transparent) using the Inspector's default duration.
        /// </summary>
        /// <param name="onComplete">Optional callback invoked when the overlay reaches full transparency.</param>
        public void FadeIn(Action onComplete = null)
        {
            FadeIn(_defaultDuration, onComplete);
        }

        /// <summary>
        /// Fade the screen in (fade color → transparent) over <paramref name="duration"/> seconds.
        /// </summary>
        /// <param name="duration">Fade duration in seconds.</param>
        /// <param name="onComplete">Optional callback invoked when the overlay reaches full transparency.</param>
        public void FadeIn(float duration, Action onComplete = null)
        {
            BeginFade(1f, 0f, duration, () =>
            {
                _canvasGroup.blocksRaycasts = false;
                OnFadeInComplete.Invoke();
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Convenience method: fade out → hold → fade in, all using the default fade duration.
        /// Ideal for scene transitions and death sequences.
        /// </summary>
        /// <param name="holdDuration">Seconds to hold the fully-opaque screen before fading back in.
        /// Respects <see cref="_useUnscaledTime"/>.</param>
        /// <param name="onComplete">Optional callback fired after the final fade-in completes.</param>
        public void FadeOutHoldIn(float holdDuration = 0f, Action onComplete = null)
        {
            FadeOut(onComplete: () => StartCoroutine(HoldThenFadeIn(holdDuration, onComplete)));
        }

        /// <summary>
        /// Update the fade color. The change takes effect on the next call to FadeOut.
        /// </summary>
        public void SetFadeColor(Color color)
        {
            _fadeColor = color;
            if (_overlay != null)
            {
                Color c = color;
                c.a = 1f;
                _overlay.color = c;
            }
        }

        /// <summary>
        /// Snap the overlay to fully opaque immediately without animation, cancelling any fade in progress.
        /// </summary>
        public void SnapOpaque()
        {
            CancelActiveFade();
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Snap the overlay to fully transparent immediately without animation, cancelling any fade in progress.
        /// </summary>
        public void SnapTransparent()
        {
            CancelActiveFade();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        // ─── Internal ─────────────────────────────────────────────────────────

        private void BeginFade(float fromAlpha, float toAlpha, float duration, Action onComplete)
        {
            CancelActiveFade();
            _activeCoroutine = StartCoroutine(FadeRoutine(fromAlpha, toAlpha, duration, onComplete));
        }

        private void CancelActiveFade()
        {
            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
                _activeCoroutine = null;
            }
            IsFading = false;
        }

        private IEnumerator FadeRoutine(float fromAlpha, float toAlpha, float duration, Action onComplete)
        {
            IsFading = true;
            _canvasGroup.alpha = fromAlpha;

            if (duration <= 0f)
            {
                _canvasGroup.alpha = toAlpha;
                IsFading = false;
                _activeCoroutine = null;
                onComplete?.Invoke();
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, _fadeCurve.Evaluate(t));
                yield return null;
            }

            _canvasGroup.alpha = toAlpha;
            IsFading = false;
            _activeCoroutine = null;
            onComplete?.Invoke();
        }

        private IEnumerator HoldThenFadeIn(float holdDuration, Action onComplete)
        {
            if (holdDuration > 0f)
            {
                if (_useUnscaledTime)
                    yield return new WaitForSecondsRealtime(holdDuration);
                else
                    yield return new WaitForSeconds(holdDuration);
            }

            FadeIn(onComplete: onComplete);
        }
    }
}
