using System.Collections.Generic;
using System.Linq;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBalls.Factories
{
    public class TileFactory
    {
        private const int ScaleStep = 2;
        public readonly int StartWidth = 7;

        private readonly IObjectResolverProvider _resolverProvider;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IPoolService _poolService;

        private List<Tile> _tilePrefabs;
        private List<Tile> _activeTiles = new List<Tile>();

        public TileFactory (IObjectResolverProvider resolverProvider, ICoroutineRunner coroutineRunner, IPoolService poolService)
        {
            _resolverProvider = resolverProvider;
            _coroutineRunner = coroutineRunner;
            _poolService = poolService;
        }

        public float TileLength => 15;
        public IReadOnlyList<Tile> ActiveTiles => _activeTiles;
        public int RoadWidth { get; private set; }

        public void StartSpawn(LevelConfig levelConfig)
        {
            RoadWidth = StartWidth;
            _tilePrefabs = levelConfig.TilePrefabs;
            SpawnNextTile(9);
            SpawnNextTile();
            SpawnNextTile();
        }

        public void SpawnNextTile(float offsetZ = 0)
        {
            Tile prefab = _tilePrefabs[Random.Range(0, _tilePrefabs.Count)];
            Vector3 spawnPoint = new Vector3(-7, -0.5f, GetLNextSpawnPointZ() - offsetZ);

            Tile newTile = _poolService.GetObject<Tile>(prefab.name);
            newTile.transform.position = spawnPoint;
            newTile.transform.rotation = Quaternion.identity;
            newTile.Construct(RoadWidth);
            newTile.Finished += () => SpawnNextTile();

            _activeTiles.Add(newTile);
        }

        public int ScaleRoad()
        {
            RoadWidth += ScaleStep;

            foreach (var tile in _activeTiles)
            {
                tile.ScaleTile(RoadWidth);
            }

            return RoadWidth;
        }

        public float GetLNextSpawnPointZ()
        {
            if (_activeTiles.Count == 0)
                return 0;

            Tile lastTile = _activeTiles[_activeTiles.Count - 1];
            return lastTile.transform.position.z + 15;
        }

        public void RemoveTile(int index)
        {
            Tile tile = _activeTiles[index];
            _poolService.ReleaseObject(tile);
            tile.Finished -= () => SpawnNextTile();

            _activeTiles.RemoveAt(index);
        }
    }
}