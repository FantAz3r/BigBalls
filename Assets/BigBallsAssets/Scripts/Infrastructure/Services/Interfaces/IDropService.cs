using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

public interface IDropService
{
    void DropLoot(Enemy enemy);
    void SetCurrentLevelConfig(List<EnemyConfig> currentEnemyConfig);
}
