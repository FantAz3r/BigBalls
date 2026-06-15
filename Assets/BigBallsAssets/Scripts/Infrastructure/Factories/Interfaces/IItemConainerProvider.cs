using BigBalls.StaticData;

namespace BigBalls.Providers
{
    public interface IItemConainerProvider
    {
        ItemStruct Gun { get; }
        ItemStruct Helmet { get; }
        ItemStruct Armor { get; }
    }
}