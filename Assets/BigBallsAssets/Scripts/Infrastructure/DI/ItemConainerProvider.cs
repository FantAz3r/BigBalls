using System;

namespace BigBalls.Providers
{
    public class ItemConainerProvider : IItemConainerProvider
    {
        public WeaponModel Gun { get; private set; }

        public HelmetModel Helmet { get; private set; }

        public ArmorModel Armor { get; private set; }

        public event Action<WeaponModel> WeaponSeted;
        public event Action<ArmorModel> ArmorSeted;
        public event Action<HelmetModel> HelmetSeted;

        public event Action<WeaponModel> WeaponRemoved;
        public event Action<ArmorModel>  ArmorRemoved;
        public event Action<HelmetModel> HelmetRemoved;

        public void Add (ICardModel item)
        {
            if (item is WeaponModel weapon)
            {
                Gun = weapon;
                WeaponSeted?.Invoke(Gun);

            }
            else if (item is HelmetModel helmet)
            {
                Helmet = helmet;
                HelmetSeted?.Invoke(Helmet);
            }
            else if (item is ArmorModel armor)
            {
                Armor = armor;
                ArmorSeted?.Invoke(Armor);
            }

            item.AddToPlayer();
        }

        public void Remove (ICardModel item)
        {
            if (item is WeaponModel)
            {
                WeaponRemoved?.Invoke(Gun);
                Gun = null;

            }
            else if (item is HelmetModel)
            {
                HelmetRemoved?.Invoke(Helmet);
                Helmet = null;
            }
            else if (item is ArmorModel)
            {
                ArmorRemoved?.Invoke(Armor);
                Armor = null;
            }

            item.RemoveFromPlayer();
        }
    }
}