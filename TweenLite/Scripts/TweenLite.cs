using System;
using UnityEngine;

namespace ShahvaizJ.TweenLite
{
    /// <summary>
    /// Tiny code-driven tween helper — no scene setup, no external package. Call one of the
    /// factory methods to get back a <see cref="Tween"/>, configure it with the fluent
    /// <c>SetEase</c> / <c>SetLoops</c> / <c>OnComplete</c> methods, and it starts advancing on
    /// the next frame automatically.
    /// <para>
    /// For common cases, prefer the extension methods in <see cref="TweenExtensions"/>
    /// (e.g. <c>transform.TweenMove(...)</c>) — they read more naturally at call sites.
    /// </para>
    /// </summary>
    public static class TweenLite
    {
        /// <summary>Tweens a raw float from <paramref name="from"/> to <paramref name="to"/>, reporting progress via <paramref name="onUpdate"/>.</summary>
        public static Tween To(float from, float to, float duration, Action<float> onUpdate)
        {
            var tween = new Tween(duration, t => onUpdate?.Invoke(Mathf.LerpUnclamped(from, to, t)));
            TweenRunner.Instance.Register(tween);
            return tween;
        }

        /// <summary>Tweens a <see cref="Color"/> from <paramref name="from"/> to <paramref name="to"/>, reporting progress via <paramref name="onUpdate"/>.</summary>
        public static Tween Color(Color from, Color to, float duration, Action<Color> onUpdate)
        {
            var tween = new Tween(duration, t => onUpdate?.Invoke(UnityEngine.Color.LerpUnclamped(from, to, t)));
            TweenRunner.Instance.Register(tween);
            return tween;
        }

        /// <summary>Moves <paramref name="target"/> to <paramref name="endValue"/> over <paramref name="duration"/> seconds.</summary>
        /// <param name="useLocalPosition">If true, tweens <see cref="Transform.localPosition"/> instead of world position.</param>
        public static Tween Move(Transform target, Vector3 endValue, float duration, bool useLocalPosition = false)
        {
            Vector3 startValue = useLocalPosition ? target.localPosition : target.position;
            var tween = new Tween(duration, t =>
            {
                Vector3 value = Vector3.LerpUnclamped(startValue, endValue, t);
                if (useLocalPosition)
                    target.localPosition = value;
                else
                    target.position = value;
            });
            TweenRunner.Instance.Register(tween);
            return tween;
        }

        /// <summary>Scales <paramref name="target"/>'s <see cref="Transform.localScale"/> to <paramref name="endValue"/> over <paramref name="duration"/> seconds.</summary>
        public static Tween Scale(Transform target, Vector3 endValue, float duration)
        {
            Vector3 startValue = target.localScale;
            var tween = new Tween(duration, t => target.localScale = Vector3.LerpUnclamped(startValue, endValue, t));
            TweenRunner.Instance.Register(tween);
            return tween;
        }

        /// <summary>Rotates <paramref name="target"/> to <paramref name="endValueEulerAngles"/> over <paramref name="duration"/> seconds.</summary>
        /// <param name="useLocalRotation">If true, tweens <see cref="Transform.localRotation"/> instead of world rotation.</param>
        public static Tween Rotate(Transform target, Vector3 endValueEulerAngles, float duration, bool useLocalRotation = true)
        {
            Quaternion startValue = useLocalRotation ? target.localRotation : target.rotation;
            Quaternion endValue = Quaternion.Euler(endValueEulerAngles);
            var tween = new Tween(duration, t =>
            {
                Quaternion value = Quaternion.SlerpUnclamped(startValue, endValue, t);
                if (useLocalRotation)
                    target.localRotation = value;
                else
                    target.rotation = value;
            });
            TweenRunner.Instance.Register(tween);
            return tween;
        }

        /// <summary>Immediately stops and removes every tween currently running.</summary>
        public static void KillAll()
        {
            TweenRunner.Instance.KillAll();
        }
    }

    /// <summary>Fluent extension methods that wrap the most common <see cref="TweenLite"/> calls.</summary>
    public static class TweenExtensions
    {
        /// <summary>Moves this transform to <paramref name="endValue"/> over <paramref name="duration"/> seconds. See <see cref="TweenLite.Move"/>.</summary>
        public static Tween TweenMove(this Transform transform, Vector3 endValue, float duration, bool useLocalPosition = false)
            => TweenLite.Move(transform, endValue, duration, useLocalPosition);

        /// <summary>Scales this transform to <paramref name="endValue"/> over <paramref name="duration"/> seconds. See <see cref="TweenLite.Scale"/>.</summary>
        public static Tween TweenScale(this Transform transform, Vector3 endValue, float duration)
            => TweenLite.Scale(transform, endValue, duration);

        /// <summary>Rotates this transform to <paramref name="endValueEulerAngles"/> over <paramref name="duration"/> seconds. See <see cref="TweenLite.Rotate"/>.</summary>
        public static Tween TweenRotate(this Transform transform, Vector3 endValueEulerAngles, float duration, bool useLocalRotation = true)
            => TweenLite.Rotate(transform, endValueEulerAngles, duration, useLocalRotation);

        /// <summary>Fades this <see cref="CanvasGroup"/>'s alpha to <paramref name="targetAlpha"/> over <paramref name="duration"/> seconds.</summary>
        public static Tween TweenFade(this CanvasGroup canvasGroup, float targetAlpha, float duration)
            => TweenLite.To(canvasGroup.alpha, targetAlpha, duration, alpha => canvasGroup.alpha = alpha);

        /// <summary>Fades this <see cref="SpriteRenderer"/>'s color alpha to <paramref name="targetAlpha"/> over <paramref name="duration"/> seconds.</summary>
        public static Tween TweenFade(this SpriteRenderer spriteRenderer, float targetAlpha, float duration)
        {
            float startAlpha = spriteRenderer.color.a;
            return TweenLite.To(startAlpha, targetAlpha, duration, alpha =>
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            });
        }

        /// <summary>Tweens this <see cref="SpriteRenderer"/>'s color to <paramref name="targetColor"/> over <paramref name="duration"/> seconds.</summary>
        public static Tween TweenColor(this SpriteRenderer spriteRenderer, Color targetColor, float duration)
            => TweenLite.Color(spriteRenderer.color, targetColor, duration, color => spriteRenderer.color = color);
    }
}
