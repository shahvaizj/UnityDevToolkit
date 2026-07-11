using System.Collections.Generic;
using UnityEngine;

namespace ShahvaizJ.ScriptableVariables
{
    /// <summary>
    /// Base class for a shared, decoupled value stored as a <see cref="ScriptableObject"/> asset.
    /// Any script or component can reference the asset directly instead of going through a
    /// singleton or manual event wiring, which keeps systems decoupled from one another.
    /// <para>
    /// The asset's <see cref="Value"/> is a separate runtime copy of <see cref="_initialValue"/> —
    /// editing the asset in play mode never touches the serialized initial value, and (when
    /// <see cref="_resetOnPlay"/> is enabled) the runtime value is reset back to the initial
    /// value every time play mode starts, so edits made during a previous play session don't
    /// leak into the next one.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of value this variable stores.</typeparam>
    public abstract class ScriptableVariable<T> : ScriptableObject
    {
        [Tooltip("Value this variable is reset to when Play Mode starts (or ResetValue is called).")]
        [SerializeField] private T _initialValue;

        [Tooltip("Automatically reset Value to Initial Value each time Play Mode starts.")]
        [SerializeField] private bool _resetOnPlay = true;

        private T _runtimeValue;
        private bool _initialized;

        /// <summary>
        /// The current runtime value. Reading it before anything has written to it returns
        /// <see cref="_initialValue"/>. Setting it invokes the subclass's change event, but only
        /// when the new value actually differs from the current one.
        /// </summary>
        public T Value
        {
            get
            {
                EnsureInitialized();
                return _runtimeValue;
            }
            set
            {
                EnsureInitialized();
                if (EqualityComparer<T>.Default.Equals(_runtimeValue, value))
                    return;

                _runtimeValue = value;
                RaiseValueChanged(_runtimeValue);
            }
        }

        /// <summary>The serialized value this variable starts from.</summary>
        public T InitialValue => _initialValue;

        protected virtual void OnEnable()
        {
            _initialized = false;

            if (_resetOnPlay)
                ResetValue();
        }

        /// <summary>Resets <see cref="Value"/> back to <see cref="InitialValue"/> without raising the change event.</summary>
        public void ResetValue()
        {
            _runtimeValue = _initialValue;
            _initialized = true;
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            _runtimeValue = _initialValue;
            _initialized = true;
        }

        /// <summary>Invoked by <see cref="Value"/>'s setter whenever the value changes. Subclasses raise their strongly-typed UnityEvent here.</summary>
        protected abstract void RaiseValueChanged(T value);

        public override string ToString() => Value.ToString();
    }
}
