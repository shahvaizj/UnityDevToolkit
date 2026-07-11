using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.ScriptableVariables
{
    /// <summary>
    /// Shared <see cref="int"/> value backed by a <see cref="ScriptableObject"/> asset. Useful for
    /// score, ammo counts, wave numbers, or any whole-number value multiple systems need to read
    /// and write without referencing each other directly.
    /// </summary>
    [CreateAssetMenu(menuName = "ShahvaizJ/Scriptable Variables/Int Variable", fileName = "NewIntVariable")]
    public class IntVariable : ScriptableVariable<int>
    {
        [Serializable] public class IntUnityEvent : UnityEvent<int> { }

        [Tooltip("Invoked whenever Value changes, passing the new value.")]
        public IntUnityEvent OnValueChanged;

        protected override void RaiseValueChanged(int value) => OnValueChanged?.Invoke(value);

        /// <summary>Adds <paramref name="amount"/> to <see cref="ScriptableVariable{T}.Value"/>.</summary>
        public void Add(int amount) => Value += amount;
    }
}
