using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.ScriptableVariables
{
    /// <summary>
    /// Shared <see cref="bool"/> value backed by a <see cref="ScriptableObject"/> asset. Useful
    /// for flags such as "game paused", "tutorial complete", or "boss unlocked" that multiple
    /// systems need to read and write without referencing each other directly.
    /// </summary>
    [CreateAssetMenu(menuName = "ShahvaizJ/Scriptable Variables/Bool Variable", fileName = "NewBoolVariable")]
    public class BoolVariable : ScriptableVariable<bool>
    {
        [Serializable] public class BoolUnityEvent : UnityEvent<bool> { }

        [Tooltip("Invoked whenever Value changes, passing the new value.")]
        public BoolUnityEvent OnValueChanged;

        protected override void RaiseValueChanged(bool value) => OnValueChanged?.Invoke(value);

        /// <summary>Flips <see cref="ScriptableVariable{T}.Value"/> to its opposite state.</summary>
        public void Toggle() => Value = !Value;
    }
}
