using BigBalls.Configs;
using BigBalls.StaticData;

namespace BigBalls.Providers
{
    public class ItemConainerProvider : IItemConainerProvider
    {
        public ItemModel Gun { get; private set; }

        public ItemModel Helmet { get; private set; }

        public ItemModel Armor { get; private set; }

        public void Set(ItemModel item)
        {
            if(item.Config is WeaponConfig)
            {
                Gun = item;
            }
            else if(item.Config is HelmetConfig)
            {
                Helmet = item;
            }
            else if(item.Config is ArmorConfig)
            {
                Armor = item;
            }
        }
    }
}