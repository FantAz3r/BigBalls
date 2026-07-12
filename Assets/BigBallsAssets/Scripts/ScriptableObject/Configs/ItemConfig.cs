using BigBalls.GameplayObjects;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    public abstract class ItemConfig : ScriptableObject, IItemConfig
    {
        [field: SerializeField] public string NameEN { get; private set; }
        [field: SerializeField] public string NameRU { get; private set; }
        [field: SerializeField] public string NameTR { get; private set; }

        [field: SerializeField] public string DescriptionEN { get; private set; }
        [field: SerializeField] public string DescriptionRU { get; private set; }
        [field: SerializeField] public string DescriptionTR { get; private set; }

        [field: SerializeField] public CardType Type { get; private set; }
        [field: SerializeField] public Rarity Rarity { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int BaseEXPForUpgrade { get; private set; } = 50;
        [field: SerializeField] public float LevelEXPMultipy { get; private set; } = 1.5f;
        [field: SerializeField] public bool IsOpen { get; private set; } = false;

        public virtual List<ItemStat> GetStats(int level) => null;
        public virtual List<BallConfig> GetBalls(int level) => null;
        public virtual List<ArtefactConfig> GetArtefacts(int level) => null;
    }
}
