using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using System.Linq;
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
        
        [field: SerializeField] public List<Vector2Int> BlocksPositions { get; private set; } = new();
        [field: SerializeField] public List<StatStruct> Stats { get; private set; } = new ();
        [field: SerializeField] public LayerMask ObstacleLayers { get; private set; }

        [field: SerializeField] public BallConfig BallConfig { get; private set; }
        
        [Header("Loot Settings")]
        [field: SerializeField] public List<LootInfo> PossibleLoot = new List<LootInfo>();
        [field: SerializeField] public int BaseExperience = 10;
        [field: SerializeField] public int BaseCoins = 5;
        
        public StatStruct Get(StatType statType) => Stats.Where(stat => stat.StatType == statType).FirstOrDefault();
        public int Weight => BlocksPositions.Count;

        public int GetWidth()
        {
            int width = 0;

            foreach(var block in BlocksPositions)
            {
                if(block.x > width)
                    width = block.x;
            }

            return width;
        }

        public List<Vector2Int> GetEnemyRow(int rowIndex)
        {
            List<Vector2Int> blocksPositionsInRow = new List<Vector2Int>();

            foreach (var block in BlocksPositions)
            {
                if (block.y == rowIndex)
                    blocksPositionsInRow.Add(block);
            }

            return blocksPositionsInRow;
        }

        public List<Vector2Int> GetBlocksWithoutFrontNeighbor()
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