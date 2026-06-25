using UnityEngine;
using UnityEngine.Events;

namespace ShahvaizJ.HealthSystem
{
    /// <summary>
    /// A flexible health component with damage, healing, death events, and configurable
    /// invincibility frames (i-frames). Attach to any GameObject that needs hit points —
    /// enemies, the player, destructible props, etc.
    /// <para>
    /// Damage is rejected while invincible. Invincibility can trigger automatically after
    /// each hit (i-frames) or be toggled manually from code. Health never drops below zero
    /// or rises above <see cref="MaxHealth"/>.
    /// </para>
    /// </summary>
    [AddComponentMenu("ShahvaizJ/Health System")]
    public class HealthSystem : MonoBehaviour
    {
        [Header("Health")]
        [Tooltip("Maximum hit points. Also used as the starting value.")]
        [SerializeField] private float _maxHealth = 100f;

        [Tooltip("If true, health resets to max when the component is enabled.")]
        [SerializeField] private bool _resetOnEnable = true;

        [Header("Invincibility Frames")]
        [Tooltip("Duration of automatic invincibility after taking damage, in seconds. Set to 0 to disable i-frames.")]
        [SerializeField] private float _iFrameDuration = 0f;

        [Tooltip("Use unscaled time for i-frames so they work while the game is paused.")]
        [SerializeField] private bool _useUnscaledTime = false;

        [Header("Events")]
        [Tooltip("Fired when damage is applied. Float parameter is the actual damage dealt.")]
        public UnityEvent<float> OnDamaged;

        [Tooltip("Fired when healing is applied. Float parameter is the actual amount healed.")]
        public UnityEvent<float> OnHealed;

        [Tooltip("Fired whenever current health changes. Float parameter is the new health value.")]
        public UnityEvent<float> OnHealthChanged;

        [Tooltip("Fired once when health reaches zero.")]
        public UnityEvent OnDeath;

        [Tooltip("Fired when invincibility begins (either from i-frames or manual toggle).")]
        public UnityEvent OnInvincibilityStarted;

        [Tooltip("Fired when invincibility ends.")]
        public UnityEvent OnInvincibilityEnded;

        /// <summary>Current health value, clamped between 0 and <see cref="MaxHealth"/>.</summary>
        public float CurrentHealth => _currentHealth;

        /// <summary>Maximum health value.</summary>
        public float MaxHealth => _maxHealth;

        /// <summary>Current health as a 0–1 fraction of max health.</summary>
        public float HealthPercent => _maxHealth > 0f ? _currentHealth / _maxHealth : 0f;

        /// <summary>True while current health is above zero.</summary>
        public bool IsAlive => _currentHealth > 0f;

        /// <summary>True while the entity is invincible (i-frames or manual).</summary>
        public bool IsInvincible => _isInvincible;

        private float _currentHealth;
        private bool _isInvincible;
        private float _iFrameTimer;
        private bool _isDead;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void OnEnable()
        {
            if (_resetOnEnable)
            {
                _currentHealth = _maxHealth;
                _isInvincible = false;
                _iFrameTimer = 0f;
                _isDead = false;
            }
        }

        private void Update()
        {
            if (_iFrameTimer <= 0f)
                return;

            float deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _iFrameTimer -= deltaTime;

            if (_iFrameTimer <= 0f)
            {
                _iFrameTimer = 0f;
                _isInvincible = false;
                OnInvincibilityEnded.Invoke();
            }
        }

        /// <summary>
        /// Applies damage to this entity. Damage is ignored while invincible or already dead.
        /// Negative values are clamped to zero.
        /// </summary>
        /// <param name="amount">Amount of damage to deal.</param>
        /// <returns>The actual damage dealt after clamping.</returns>
        public float TakeDamage(float amount)
        {
            if (_isDead || _isInvincible)
                return 0f;

            amount = Mathf.Max(amount, 0f);
            if (amount <= 0f)
                return 0f;

            float actualDamage = Mathf.Min(amount, _currentHealth);
            _currentHealth -= actualDamage;

            OnDamaged.Invoke(actualDamage);
            OnHealthChanged.Invoke(_currentHealth);

            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                _isDead = true;
                OnDeath.Invoke();
                return actualDamage;
            }

            if (_iFrameDuration > 0f)
            {
                _isInvincible = true;
                _iFrameTimer = _iFrameDuration;
                OnInvincibilityStarted.Invoke();
            }

            return actualDamage;
        }

        /// <summary>
        /// Heals the entity. Cannot exceed max health. Healing a dead entity has no effect
        /// unless it is revived first with <see cref="Revive"/>.
        /// Negative values are clamped to zero.
        /// </summary>
        /// <param name="amount">Amount of health to restore.</param>
        /// <returns>The actual amount healed after clamping.</returns>
        public float Heal(float amount)
        {
            if (_isDead)
                return 0f;

            amount = Mathf.Max(amount, 0f);
            if (amount <= 0f)
                return 0f;

            float actualHeal = Mathf.Min(amount, _maxHealth - _currentHealth);
            if (actualHeal <= 0f)
                return 0f;

            _currentHealth += actualHeal;

            OnHealed.Invoke(actualHeal);
            OnHealthChanged.Invoke(_currentHealth);

            return actualHeal;
        }

        /// <summary>
        /// Instantly kills the entity, setting health to zero and firing <see cref="OnDeath"/>.
        /// Ignores invincibility.
        /// </summary>
        public void Kill()
        {
            if (_isDead)
                return;

            float previousHealth = _currentHealth;
            _currentHealth = 0f;
            _isDead = true;

            ClearInvincibility();

            if (previousHealth > 0f)
                OnDamaged.Invoke(previousHealth);

            OnHealthChanged.Invoke(0f);
            OnDeath.Invoke();
        }

        /// <summary>
        /// Revives the entity with the specified health. Clears the dead state so healing
        /// and damage work again.
        /// </summary>
        /// <param name="healthAmount">Health to revive with. Clamped to 1–MaxHealth.</param>
        public void Revive(float healthAmount)
        {
            healthAmount = Mathf.Clamp(healthAmount, 1f, _maxHealth);
            _currentHealth = healthAmount;
            _isDead = false;

            ClearInvincibility();
            OnHealthChanged.Invoke(_currentHealth);
        }

        /// <summary>
        /// Revives the entity at full health.
        /// </summary>
        public void Revive()
        {
            Revive(_maxHealth);
        }

        /// <summary>
        /// Resets health to max without firing damage/death events. Clears dead and
        /// invincibility states.
        /// </summary>
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            _isDead = false;
            ClearInvincibility();
            OnHealthChanged.Invoke(_currentHealth);
        }

        /// <summary>
        /// Sets max health to a new value. Current health is clamped to the new max.
        /// If clamping causes death, <see cref="OnDeath"/> fires.
        /// </summary>
        /// <param name="newMax">The new maximum health. Must be greater than zero.</param>
        public void SetMaxHealth(float newMax)
        {
            _maxHealth = Mathf.Max(newMax, 1f);

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
                OnHealthChanged.Invoke(_currentHealth);
            }
        }

        /// <summary>
        /// Manually enables invincibility for the specified duration. Replaces any active
        /// i-frame timer if the new duration is longer.
        /// </summary>
        /// <param name="duration">Duration in seconds. Pass <see cref="Mathf.Infinity"/>
        /// for permanent invincibility until manually cleared.</param>
        public void SetInvincible(float duration)
        {
            if (duration <= 0f)
                return;

            bool wasInvincible = _isInvincible;
            _isInvincible = true;

            if (!float.IsPositiveInfinity(duration))
            {
                _iFrameTimer = Mathf.Max(_iFrameTimer, duration);
            }
            else
            {
                _iFrameTimer = 0f;
            }

            if (!wasInvincible)
                OnInvincibilityStarted.Invoke();
        }

        /// <summary>
        /// Immediately clears invincibility, whether it came from i-frames or a manual call.
        /// </summary>
        public void ClearInvincibility()
        {
            if (!_isInvincible)
                return;

            _isInvincible = false;
            _iFrameTimer = 0f;
            OnInvincibilityEnded.Invoke();
        }
    }
}