using System.Diagnostics;
using UnityEngine;

namespace ShahvaizJ.CustomLog
{
    /// <summary>
    /// Conditional debug logger. All calls are stripped in non-development builds.
    /// </summary>
    public static class Log
    {
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Print(string message, Object context = null) =>
            UnityEngine.Debug.Log(message, context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Warning(string message, Object context = null) =>
            UnityEngine.Debug.LogWarning(message, context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Error(string message, Object context = null) =>
            UnityEngine.Debug.LogError(message, context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Blue(string message, Object context = null) =>
            UnityEngine.Debug.Log($"<color=#4FC3F7>{message}</color>", context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Red(string message, Object context = null) =>
            UnityEngine.Debug.Log($"<color=#EF5350>{message}</color>", context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Yellow(string message, Object context = null) =>
            UnityEngine.Debug.Log($"<color=#FFEE58>{message}</color>", context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Green(string message, Object context = null) =>
            UnityEngine.Debug.Log($"<color=#66BB6A>{message}</color>", context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Cyan(string message, Object context = null) =>
            UnityEngine.Debug.Log($"<color=#26C6DA>{message}</color>", context);

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public static void Orange(string message, Object context = null) =>
            UnityEngine.Debug.Log($"<color=#FFA726>{message}</color>", context);
    }
}
