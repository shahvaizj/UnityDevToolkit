using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.ScriptableVariables
{
    /// <summary>
    /// Shared <see cref="string"/> value backed by a <see cref="ScriptableObject"/> asset. Useful
    /// for player names, current objective text, or locale keys multiple systems need to read
    /// and write without referencing each other directly.
    /// </summary>
    [CreateAssetMenu(menuName = "ShahvaizJ/Scriptable Variables/String Variable", fileName = "NewStringVariable")]
    public class StringVariable : ScriptableVariable<string>
    {
        [Serializable] public class StringUnityEvent : UnityEvent<string> { }

        [Tooltip("Invoked whenever Value changes, passing the new value.")]
        public StringUnityEvent OnValueChanged;

        protected override void RaiseValueChanged(string value) => OnValueChanged?.Invoke(value);
    }
}
