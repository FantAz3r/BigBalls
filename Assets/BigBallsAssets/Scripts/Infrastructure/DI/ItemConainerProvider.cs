using BigBalls.Configs;
using BigBalls.StaticData;

namespace BigBalls.Providers
{
    public class ItemConainerProvider : IItemConainerProvider
    {
        public ItemStruct Gun { get; private set; }

        public ItemStruct Helmet { get; private set; }

        public ItemStruct Armor { get; private set; }

        public void Set(ItemStruct item)
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