using BigBalls.Factories;
using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class TileMover : IUpdateble
    {
        private const float TileMoveSpeed = 0.2f;

        private readonly TileFactory _factory;
        private readonly IUpdateService _updateService;

        public TileMover(TileFactory factory, IUpdateService updateService)
        {
            _factory = factory;
            _updateService = updateService;
        }

        public void Start() => _updateService.Register(this);

        public void Stop() => _updateService.Unregister(this);

        public void Tick() => MoveTiles();

        private void MoveTiles()
        {
            var activeTiles = _factory.ActiveTiles;

            for (int i = activeTiles.Count - 1; i >= 0; i--)
            {
                Tile tile = activeTiles[i];
                tile.transform.Translate(Vector3.back * TileMoveSpeed * Time.deltaTime);

                if (tile.transform.position.z <= -_factory.TileLength * 2)
                {
                    _factory.RemoveTile(i);
                }
            }
        }
    }
}