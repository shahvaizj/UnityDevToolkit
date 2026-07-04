using System;
using UnityEngine;

namespace ShahvaizJ.TweenLite
{
    /// <summary>How a looping <see cref="Tween"/> behaves once it reaches the end of a cycle.</summary>
    public enum LoopType
    {
        /// <summary>Jump back to the start value and play forward again.</summary>
        Restart,

        /// <summary>Reverse direction each cycle, bouncing between start and end.</summary>
        PingPong
    }

    /// <summary>
    /// A single running interpolation created by <see cref="TweenLite"/>. Configure it with the
    /// fluent <c>SetX</c> methods immediately after creation, before the next frame ticks it.
    /// <para>
    /// Instances are driven internally by a hidden runner GameObject — there is nothing to add to
    /// a scene and nothing to update manually.
    /// </para>
    /// </summary>
    public sealed class Tween
    {
        /// <summary>True while this tween is still running (not completed or killed).</summary>
        public bool IsActive { get; private set; } = true;

        /// <summary>True while ticking is temporarily suspended via <see cref="Pause"/>.</summary>
        public bool IsPaused { get; private set; }

        /// <summary>Length of a single cycle, in seconds.</summary>
        public float Duration { get; }

        /// <summary>Whether this tween advances with <see cref="Time.unscaledDeltaTime"/> instead of scaled time.</summary>
        public bool UseUnscaledTime { get; private set; }

        private readonly Action<float> _onUpdate;
        private Action _onComplete;
        private Action _onKill;

        private Ease _ease = Ease.Linear;
        private float _delayRemaining;
        private float _elapsed;
        private int _loopsRemaining = 1;
        private LoopType _loopType = LoopType.Restart;
        private bool _playingForward = true;

        internal Tween(float duration, Action<float> onUpdate)
        {
            Duration = Mathf.Max(0f, duration);
            _onUpdate = onUpdate;
        }

        /// <summary>Sets the easing curve applied to the tween's progress. Default is <see cref="Ease.Linear"/>.</summary>
        public Tween SetEase(Ease ease)
        {
            _ease = ease;
            return this;
        }

        /// <summary>Delays the first update by <paramref name="seconds"/> before the tween starts advancing.</summary>
        public Tween SetDelay(float seconds)
        {
            _delayRemaining = Mathf.Max(0f, seconds);
            return this;
        }

        /// <summary>
        /// Repeats the tween. Pass a negative <paramref name="loops"/> to repeat forever.
        /// Default is 1 (play once, no repeat).
        /// </summary>
        public Tween SetLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            _loopsRemaining = loops;
            _loopType = loopType;
            return this;
        }

        /// <summary>Advances this tween with unscaled time, so it keeps animating while <see cref="Time.timeScale"/> is 0.</summary>
        public Tween SetUnscaledTime(bool unscaled = true)
        {
            UseUnscaledTime = unscaled;
            return this;
        }

        /// <summary>Registers a callback invoked once when the tween finishes all of its loops.</summary>
        public Tween OnComplete(Action callback)
        {
            _onComplete = callback;
            return this;
        }

        /// <summary>Registers a callback invoked when the tween is stopped early via <see cref="Kill"/>.</summary>
        public Tween OnKill(Action callback)
        {
            _onKill = callback;
            return this;
        }

        /// <summary>Suspends ticking. The tween keeps its current progress and can be resumed with <see cref="Play"/>.</summary>
        public void Pause() => IsPaused = true;

        /// <summary>Resumes ticking after a <see cref="Pause"/>.</summary>
        public void Play() => IsPaused = false;

        /// <summary>
        /// Stops the tween immediately and unregisters it from the runner.
        /// </summary>
        /// <param name="triggerOnComplete">If true, the <see cref="OnComplete"/> callback fires as if the tween had finished naturally.</param>
        public void Kill(bool triggerOnComplete = false)
        {
            if (!IsActive)
                return;

            IsActive = false;

            if (triggerOnComplete)
                _onComplete?.Invoke();

            _onKill?.Invoke();
        }

        internal void Tick(float deltaTime)
        {
            if (!IsActive || IsPaused)
                return;

            if (_delayRemaining > 0f)
            {
                _delayRemaining -= deltaTime;
                if (_delayRemaining > 0f)
                    return;

                // Carry the leftover delta into this frame's progress instead of dropping it.
                deltaTime = -_delayRemaining;
                _delayRemaining = 0f;
            }

            _elapsed += deltaTime;

            float linearT = Duration > 0f ? Mathf.Clamp01(_elapsed / Duration) : 1f;
            float directionalT = _playingForward ? linearT : 1f - linearT;
            _onUpdate?.Invoke(Easing.Evaluate(_ease, directionalT));

            if (_elapsed >= Duration)
                AdvanceLoop();
        }

        private void AdvanceLoop()
        {
            bool infinite = _loopsRemaining < 0;

            if (!infinite)
                _loopsRemaining--;

            if (!infinite && _loopsRemaining <= 0)
            {
                IsActive = false;
                _onComplete?.Invoke();
                return;
            }

            _elapsed = 0f;

            if (_loopType == LoopType.PingPong)
                _playingForward = !_playingForward;
        }
    }
}
