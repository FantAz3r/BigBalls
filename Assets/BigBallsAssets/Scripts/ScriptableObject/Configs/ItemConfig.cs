using UnityEngine;

namespace BigBalls.Configs
{
    public abstract class ItemConfig : ScriptableObject, IItemConfig
    {
        [field: SerializeField] public Rarity Rarity { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: SerializeField] public string NameEN { get; private set; }
        [field: SerializeField] public string NameRU { get; private set; }
        [field: SerializeField] public string NameTR { get; private set; }

        [field: SerializeField] public string DescriptionEN { get; private set; }
        [field: SerializeField] public string DescriptionRU { get; private set; }
        [field: SerializeField] public string DescriptionTR { get; private set; }
    }
}
