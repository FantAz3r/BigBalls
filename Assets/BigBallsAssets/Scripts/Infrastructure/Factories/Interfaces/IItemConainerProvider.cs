using BigBalls.Configs;
using BigBalls.StaticData;

namespace BigBalls.Providers
{
    public interface IItemConainerProvider
    {
        IWeapon Gun { get; }
        IBuffer Helmet { get; }
        IArmor Armor { get; }
    }
}