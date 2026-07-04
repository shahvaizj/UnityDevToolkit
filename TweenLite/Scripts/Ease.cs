using UnityEngine;

namespace ShahvaizJ.TweenLite
{
    /// <summary>Easing curves available to every <see cref="Tween"/>.</summary>
    public enum Ease
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InBack,
        OutBack,
        InOutBack
    }

    // Robert Penner-style easing formulas, normalized to a 0-1 time and 0-1 value range.
    internal static class Easing
    {
        private const float BackOvershoot = 1.70158f;
        private const float BackOvershootInOut = BackOvershoot * 1.525f;
        private const float BackOvershootPlusOne = BackOvershoot + 1f;

        internal static float Evaluate(Ease ease, float t)
        {
            t = Mathf.Clamp01(t);

            switch (ease)
            {
                case Ease.Linear:
                    return t;
                case Ease.InSine:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                case Ease.OutSine:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);
                case Ease.InOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
                case Ease.InQuad:
                    return t * t;
                case Ease.OutQuad:
                    return 1f - (1f - t) * (1f - t);
                case Ease.InOutQuad:
                    return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
                case Ease.InCubic:
                    return t * t * t;
                case Ease.OutCubic:
                    return 1f - Mathf.Pow(1f - t, 3f);
                case Ease.InOutCubic:
                    return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
                case Ease.InBack:
                    return BackOvershootPlusOne * t * t * t - BackOvershoot * t * t;
                case Ease.OutBack:
                    {
                        float shifted = t - 1f;
                        return 1f + BackOvershootPlusOne * shifted * shifted * shifted + BackOvershoot * shifted * shifted;
                    }
                case Ease.InOutBack:
                    return t < 0.5f
                        ? Mathf.Pow(2f * t, 2f) * ((BackOvershootInOut + 1f) * 2f * t - BackOvershootInOut) * 0.5f
                        : (Mathf.Pow(2f * t - 2f, 2f) * ((BackOvershootInOut + 1f) * (t * 2f - 2f) + BackOvershootInOut) + 2f) * 0.5f;
                default:
                    return t;
            }
        }
    }
}
