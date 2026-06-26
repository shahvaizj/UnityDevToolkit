namespace ShahvaizJ.ObjectPooler
{
    /// <summary>
    /// Implement on any MonoBehaviour that lives in a pool. The pooler calls these
    /// methods instead of Instantiate/Destroy so you can reset state without
    /// allocation overhead.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when the object is pulled from the pool and activated.
        /// Use this to initialize or reset runtime state — treat it like <c>Start</c>.
        /// </summary>
        void OnSpawnFromPool();

        /// <summary>
        /// Called just before the object is deactivated and returned to the pool.
        /// Use this to cancel coroutines, release references, or stop effects.
        /// </summary>
        void OnReturnToPool();
    }
}
