using System.Collections.Generic;
using System.Linq;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]

    public class EnemyConfig : ScriptableObject, IEntityConfig
    {
        [field: SerializeField] public Enemy Prefab { get; private set; }
        [field: SerializeField] public EntityType Type { get; private set; }
        [field: SerializeField] public bool CanSuiside { get; private set; }
        [field: SerializeField] public bool HasRangeAttack { get; private set; }
        [field: SerializeField] public bool HasMeeleAttack { get; private set; }
        [field: SerializeField] public SoundStruct DieSound { get; private set; }
        [field: SerializeField] public SoundStruct AttackSound { get; private set; }
        [field: SerializeField] public SoundStruct RangeAttackSound { get; private set; }

        [field: SerializeField] public List<Vector2Int> BlocksPositions { get; private set; } = new();
        [field: SerializeField] public List<StatStruct> Stats { get; private set; } = new();
        [field: SerializeField] public LayerMask ObstacleLayers { get; private set; }

        [field: SerializeField] public BallConfig BallConfig { get; private set; }
        [field: SerializeField] public AudioClip ShootSound { get; private set; }
        [field: SerializeField] public List<EffectType> DebuffResists { get; private set; } = new();

        [Header("Loot Settings")]
        [field: SerializeField] public List<LootInfo> PossibleLoot = new List<LootInfo>();
        [field: SerializeField] public int BaseExperience = 10;
        [field: SerializeField] public int BaseCoins = 5;

        public StatStruct Get (StatType statType) => Stats.Where(stat => stat.StatType == statType).FirstOrDefault();
        public int Weight => BlocksPositions.Count;

        public int GetWidth ()
        {
            if (BlocksPositions == null || BlocksPositions.Count == 0)
                return 0;

            int minX = int.MaxValue;
            int maxX = int.MinValue;

            foreach (var block in BlocksPositions)
            {
                if (block.x < minX) minX = block.x;
                if (block.x > maxX) maxX = block.x;
            }

            return maxX - minX;
        }

        public List<Vector2Int> GetEnemyRow (int rowIndex)
        {
            List<Vector2Int> blocksPositionsInRow = new List<Vector2Int>();

            foreach (var block in BlocksPositions)
            {
                if (block.y == rowIndex)
                    blocksPositionsInRow.Add(block);
            }

            return blocksPositionsInRow;
        }

        public List<Vector2Int> GetBlocksWithoutFrontNeighbor ()
        {
            List<Vector2Int> result = new List<Vector2Int>();

            foreach (var block in BlocksPositions)
            {
                Vector2Int frontNeighbor = new Vector2Int(block.x, block.y - 1);

                if (BlocksPositions.Contains(frontNeighbor) == false)
                {
                    result.Add(block);
                }
            }
            return result;
        }
    }
}