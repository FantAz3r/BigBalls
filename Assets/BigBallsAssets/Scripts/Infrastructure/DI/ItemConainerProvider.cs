namespace BigBalls.Providers
{
    public class ItemConainerProvider : IItemConainerProvider
    {
        public WeaponModel Gun { get; private set; }

        public HelmetModel Helmet { get; private set; }

        public ArmorModel Armor { get; private set; }

        public void Add(ICardModel item)
        {
            if (item is WeaponModel weapon)
            {
                Gun = weapon;
            }
            else if (item is HelmetModel helmet)
            {
                Helmet = helmet;
            }
            else if (item is ArmorModel armor)
            {
                Armor = armor;
            }
        }

        public void Remove(ICardModel item)
        {
            if (item is WeaponModel)
            {
                Gun = null;
            }
            else if (item is HelmetModel)
            {
                Helmet = null;
            }
            else if (item is ArmorModel)
            {
                Armor = null;
            }
        }
    }
}