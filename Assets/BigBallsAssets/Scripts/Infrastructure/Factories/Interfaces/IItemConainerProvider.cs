using System;

namespace BigBalls.Providers
{
    public interface IItemConainerProvider
    {
        WeaponModel Gun { get; }
        HelmetModel Helmet { get; }
        ArmorModel Armor { get; }

        event Action<WeaponModel> WeaponSeted;
        event Action<ArmorModel> ArmorSeted;
        event Action<HelmetModel> HelmetSeted;

        event Action <WeaponModel> WeaponRemoved;
        event Action <ArmorModel> ArmorRemoved;
        event Action <HelmetModel> HelmetRemoved;

        void Add (ICardModel item);

        void Remove (ICardModel item);
    }
}