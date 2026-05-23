using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class TileGenerator : IUpdateble
    {
        private const int TileLength = 50;
        private const int TileMoveSpeed = 1;

        private readonly IObjectResolverProvider _resolverProvider;
        private readonly IUpdateService _updateService;
        private readonly List<Tile> _activeTiles = new List<Tile>();

        private List<Tile> _tiles;

        public TileGenerator(IObjectResolverProvider resolverProvider, IUpdateService updateService)
        {
            _resolverProvider = resolverProvider;
            _updateService = updateService;
        }

        public void StartSpawn(LevelConfig levelConfig)
        {
            _tiles = levelConfig.TilePrefabs;
            CheckAndSpawn();
            _updateService.Register(this);
        }

        public void Tick()
        {
            MoveTiles();
        }

        private void MoveTiles()
        {
            for (int i = _activeTiles.Count - 1; i >= 0; i--)
            {
                Tile tile = _activeTiles[i];
                tile.transform.Translate(Vector3.back * TileMoveSpeed * Time.deltaTime);

                if (tile.transform.position.z <= -TileLength)
                {
                    RemoveTile(i);
                }
            }
        }

        private void CheckAndSpawn()
        {
            if (_activeTiles.Count == 0 || GetLastTileZ() < TileLength * (_activeTiles.Count - 1))
            {
                SpawnNextTile();
            }
        }

        private void SpawnNextTile()
        {
            Tile tile = _tiles[Random.Range(0, _tiles.Count)];

            float spawnZ = _activeTiles.Count == 0 ? 0 : GetLastTileZ() + TileLength;
            Vector3 spawnPoint = new Vector3(0, 0, spawnZ);

            Tile newTile = _resolverProvider.CurrentResolver.Instantiate(tile, spawnPoint, Quaternion.identity);
            newTile.Finished += SpawnNextTile;
            _activeTiles.Add(newTile);
        }

        private float GetLastTileZ()
        {
            if (_activeTiles.Count == 0) return 0;
            return _activeTiles[_activeTiles.Count - 1].transform.position.z;
        }

        private void RemoveTile(int index)
        {
            Tile tile = _activeTiles[index];
            tile.Finished -= SpawnNextTile;
            tile.gameObject.SetActive(false);
            _activeTiles.RemoveAt(index);
        }
    }
}