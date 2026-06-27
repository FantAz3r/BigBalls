using BigBalls.StaticData;

namespace BigBalls.Providers
{
    public interface IItemConainerProvider
    {
        WeaponModel Gun { get; }
        HelmetModel Helmet { get; }
        ArmorModel Armor { get; }

        void Add(ICardModel item);
        void Remove(ICardModel item);
    }
}