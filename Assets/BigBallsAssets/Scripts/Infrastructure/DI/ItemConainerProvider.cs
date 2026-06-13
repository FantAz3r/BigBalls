using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

namespace BigBalls.Providers
{
    public class ItemConainerProvider : IItemConainerProvider
    {
        public IWeapon Gun { get; private set; }

        public IBuffer Helmet { get; private set; }

        public IArmor Armor { get; private set; }

        public void Set(IWeapon gun)
        {
            Gun = gun;
        }

        public void Set(IBuffer helmet)
        {
            Helmet = helmet;
        }

        public void Set(IArmor armor)
        {
            Armor = armor;
        }
    }
}