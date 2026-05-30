using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover
{
    private readonly Mover _mover;

    private Vector3[] _raycastPoints;
    private Vector3[] _raycastDirections;
    private EnemyConfig _enemyConfig;

    public EnemyMover(Mover mover, EnemyConfig enemyConfig)
    {
        _mover = mover;
        _enemyConfig = enemyConfig;

        _mover.SetDirection(new Vector2(0, -1));

        InitRaycasts();

        _mover.SetReycastInfo(_raycastDirections, _raycastPoints);
    }

    private void InitRaycasts()
    {
        List<Vector2Int> frontRowBlocks = _enemyConfig.GetBlocksWithoutFrontNeighbor();

        _raycastPoints = new Vector3[frontRowBlocks.Count];
        _raycastDirections = new Vector3[frontRowBlocks.Count];

        for (int i = 0; i < frontRowBlocks.Count; i++)
        {
            Vector2Int blockPos = frontRowBlocks[i];

            _raycastPoints[i] = new Vector3( blockPos.x, 0.25f, blockPos.y);
            _raycastDirections[i] = Vector3.back;
        }
    }
}
