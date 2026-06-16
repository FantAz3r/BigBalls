using BigBalls.Configs;

namespace BigBalls.StaticData
{
    public class ItemModel
    {
        public int Id;
        public ItemConfig Config;
        public int Level;

        public ItemModel(int id, ItemConfig config, int level = 1)
        {
            Id = id;
            Config = config;
            Level = level;
        }
    }
}