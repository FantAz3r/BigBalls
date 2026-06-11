using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

public interface IDropService
{
    void DropLoot(Vector3 position, Enemy enemy);
    void SetCurrentLevelConfig(List<EnemyConfig> currentEnemyConfig);
}
