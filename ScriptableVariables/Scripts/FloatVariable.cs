using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.ScriptableVariables
{
    /// <summary>
    /// Shared <see cref="float"/> value backed by a <see cref="ScriptableObject"/> asset. Useful
    /// for player health, currency, sensitivity settings, or any numeric value multiple systems
    /// need to read and write without referencing each other directly.
    /// </summary>
    [CreateAssetMenu(menuName = "ShahvaizJ/Scriptable Variables/Float Variable", fileName = "NewFloatVariable")]
    public class FloatVariable : ScriptableVariable<float>
    {
        [Serializable] public class FloatUnityEvent : UnityEvent<float> { }

        [Tooltip("Invoked whenever Value changes, passing the new value.")]
        public FloatUnityEvent OnValueChanged;

        protected override void RaiseValueChanged(float value) => OnValueChanged?.Invoke(value);

        /// <summary>Adds <paramref name="amount"/> to <see cref="ScriptableVariable{T}.Value"/>.</summary>
        public void Add(float amount) => Value += amount;
    }
}
