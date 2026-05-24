using BigBalls.Factories;
using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class TileMover : IUpdateble
    {
        private const int TileMoveSpeed = 1;

        private readonly TileFactory _factory;
        private readonly IUpdateService _updateService;

        private int _roadWidth = 7;

        public TileMover(TileFactory factory, IUpdateService updateService)
        {
            _factory = factory;
            _updateService = updateService;
        }

        public void Start()
        {
            _updateService.Register(this);
        }

        public void Tick()
        {
            MoveTiles();
            HandleInput();
        }

        private void MoveTiles()
        {
            var activeTiles = _factory.ActiveTiles;

            for (int i = activeTiles.Count - 1; i >= 0; i--)
            {
                Tile tile = activeTiles[i];
                tile.transform.Translate(Vector3.back * TileMoveSpeed * Time.deltaTime);

                if (tile.transform.position.z <= -_factory.TileLength)
                {
                    _factory.RemoveTile(i);
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _roadWidth += 2;
                ScaleRoad();
            }
        }

        private void ScaleRoad()
        {
            foreach (var tile in _factory.ActiveTiles)
            {
                tile.ScaleTile(_roadWidth);
            }
        }
    }
}