using UnityEngine;

namespace BigBalls.Configs
{
    public abstract class ItemConfig : ScriptableObject, IItem
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public Rarity Rarity { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] [field: Range(0, 10)] public int Level { get; private set; }

        [field: SerializeField] public string NameEN { get; private set; }
        [field: SerializeField] public string NameRU { get; private set; }
        [field: SerializeField] public string NameTR { get; private set; }

        [field: SerializeField] public string DescriptionEN { get; private set; }
        [field: SerializeField] public string DescriptionRU { get; private set; }
        [field: SerializeField] public string DescriptionTR { get; private set; }
    }
}
