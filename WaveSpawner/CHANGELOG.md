# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-07-08

### Added
- Initial release.
- WaveSpawner component with Inspector-configurable ordered wave list.
- WaveEntry configuration: name, prefab list, count, spawn interval, start delay, interval curve.
- Sequential and random spawn point selection via SpawnPointMode.
- StartWaves, StopWaves, and SkipToWave for runtime control.
- Optional looping back to the first wave after the last completes.
- OnWaveStarted, OnWaveCompleted, OnAllWavesCompleted, and OnUnitSpawned UnityEvents.
