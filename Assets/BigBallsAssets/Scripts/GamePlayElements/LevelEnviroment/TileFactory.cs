using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class TileFactory
    {
        private const int StartWidth = 7;

        public readonly int TileLength = 50;
        private readonly IObjectResolverProvider _resolverProvider;
        private readonly ICoroutineRunner _coroutineRunner;

        private List<Tile> _tilePrefabs;

        private List<Tile> _activeTiles = new List<Tile>();
        public IReadOnlyList<Tile> ActiveTiles => _activeTiles;

        public TileFactory(IObjectResolverProvider resolverProvider, ICoroutineRunner coroutineRunner)
        {
            _resolverProvider = resolverProvider;
            _coroutineRunner = coroutineRunner;
        }

        public void StartSpawn(LevelConfig levelConfig)
        {
            _tilePrefabs = levelConfig.TilePrefabs;
            SpawnNextTile(StartWidth);
        }

        public void SpawnNextTile(int currentRoadWidth)
        {
            Tile prefab = _tilePrefabs[Random.Range(0, _tilePrefabs.Count)];

            Vector3 spawnPoint;

            if (_activeTiles.Count == 0)
            {
                spawnPoint = new Vector3(0, 0, 0);
            }
            else
            {
                spawnPoint = new Vector3(0, 0, TileLength);
            }

            Tile newTile = _resolverProvider.CurrentResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);
            newTile.Construct(_coroutineRunner, currentRoadWidth);
            newTile.Finished += () => SpawnNextTile(currentRoadWidth);

            _activeTiles.Add(newTile);
        }

        private void CheckAndSpawn()
        {
            if (ActiveTiles.Count == 0 || GetLastTileZ() < TileLength * (ActiveTiles.Count - 1))
            {
                //SpawnNextTile(_roadWidth);
            }
        }

        public float GetLastTileZ()
        {
            if (_activeTiles.Count == 0)
                return 0;
            return _activeTiles[_activeTiles.Count - 1].transform.position.z;
        }

        public void RemoveTile(int index)
        {
            Tile tile = _activeTiles[index];
            tile.gameObject.SetActive(false);
            _activeTiles.RemoveAt(index);
        }
    }
}