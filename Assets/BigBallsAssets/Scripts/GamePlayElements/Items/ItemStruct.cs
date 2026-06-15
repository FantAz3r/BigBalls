using BigBalls.Configs;

namespace BigBalls.StaticData
{
    public struct ItemStruct
    {
        public int Id;
        public ItemConfig Config;
        public int Level;

        public ItemStruct(int id, ItemConfig config, int level = 1)
        {
            Id = id;
            Config = config;
            Level = level;
        }
    }
}