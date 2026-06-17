using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.StaticData
{
    public class ItemModel : ICard
    {
        public int Id;
        public ItemConfig Config;
        public int Level { get; private set; }

        public ItemModel (int id, ItemConfig config, int level = 1)
        {
            Id = id;
            Config = config;
            Level = level;
        }

        public Sprite Icon => Icon;

        public string Name => Config.NameRU;

        public string Description => Config.DescriptionRU;


        public void Upgrade () => Level++;
    }
}