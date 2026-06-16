using BigBalls.StaticData;

namespace BigBalls.Providers
{
    public interface IItemConainerProvider
    {
        ItemModel Gun { get; }
        ItemModel Helmet { get; }
        ItemModel Armor { get; }
    }
}