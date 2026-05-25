using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class TileFactory
    {
        private const int ScaleStep = 2;
        public readonly int StartWidth = 7;

        private readonly IObjectResolverProvider _resolverProvider;
        private readonly ICoroutineRunner _coroutineRunner;

        private List<Tile> _tilePrefabs;
        private List<Tile> _activeTiles = new List<Tile>();

        public TileFactory(IObjectResolverProvider resolverProvider, ICoroutineRunner coroutineRunner)
        {
            _resolverProvider = resolverProvider;
            _coroutineRunner = coroutineRunner;
        }

        public float TileLength => _tilePrefabs.First().transform.localScale.z;
        public IReadOnlyList<Tile> ActiveTiles => _activeTiles;
        public int RoadWidth { get; private set; }

        public void StartSpawn(LevelConfig levelConfig)
        {
            RoadWidth = StartWidth;
            _tilePrefabs = levelConfig.TilePrefabs;
            SpawnNextTile();
        }

        public void SpawnNextTile()
        {
            Tile prefab = _tilePrefabs[Random.Range(0, _tilePrefabs.Count)];
            Vector3 spawnPoint = new Vector3(0, 0, GetLNextSpawnPointZ());

            Tile newTile = _resolverProvider.CurrentResolver.Instantiate(prefab, spawnPoint, Quaternion.identity);
            newTile.Construct(_coroutineRunner, RoadWidth);
            newTile.Finished += () => SpawnNextTile();

            _activeTiles.Add(newTile);
        }

        public void ScaleRoad()
        {
            RoadWidth += ScaleStep;

            foreach (var tile in _activeTiles)
            {
                tile.ScaleTile(RoadWidth);
            }
        }

        public float GetLNextSpawnPointZ()
        {
            if (_activeTiles.Count == 0)
                return 0;

            Tile lastTile = _activeTiles[_activeTiles.Count - 1];
            return lastTile.transform.position.z + lastTile.transform.localScale.z;
        }

        public void RemoveTile(int index)
        {
            Tile tile = _activeTiles[index];
            tile.gameObject.SetActive(false);
            tile.Finished -= () => SpawnNextTile(RoadWidth);

            _activeTiles.RemoveAt(index);
        }
    }
}