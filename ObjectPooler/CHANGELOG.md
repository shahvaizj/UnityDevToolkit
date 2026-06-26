# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] – 2026-06-27

### Added
- Initial release.
- ObjectPooler component with Inspector-configurable keyed pools.
- PoolEntry configuration: key, prefab, pre-warm count, max size, auto-grow toggle.
- IPoolable interface with OnSpawnFromPool and OnReturnToPool callbacks.
- Get and Return methods for spawning and recycling objects.
- ReturnAll for bulk reclamation (all pools or by key).
- Runtime pool registration via RegisterPool.
- WarmUp method for adding instances to an existing pool.
- CountAvailable and CountActive query methods.
- OnObjectSpawned and OnObjectReturned UnityEvents.
- Automatic hierarchy parenting under the pooler transform.
