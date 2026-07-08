namespace ShahvaizJ.WaveSpawner
{
    /// <summary>
    /// Determines how <see cref="WaveSpawner"/> picks a spawn point for each unit.
    /// </summary>
    public enum SpawnPointMode
    {
        /// <summary>Cycle through spawn points in list order, wrapping around.</summary>
        Sequential,

        /// <summary>Pick a spawn point at random for every unit.</summary>
        Random
    }
}
