using BigBalls.GameplayObjects;
using BigBalls.Infrastructure;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig")]

    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public LevelID Level { get; private set; } = LevelID.Level1;
        [field: SerializeField] public List<Tile> TilePrefabs { get; private set; }
        [field: SerializeField] public List<Wave> Waves { get; private set; }
        [field: SerializeField] public List<EnemyConfig> Enemies { get; private set; }
    }
}