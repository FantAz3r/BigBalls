using BigBalls.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Infrastructure.DI
{
    public class MenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private ObjectContainer _objectContainer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_objectContainer);
            builder.RegisterEntryPoint<MenuEntryPoint>(Lifetime.Scoped);
            builder.Register<MainMenuState>(Lifetime.Scoped);
            builder.Register<ArmorRepository>(Lifetime.Scoped);
            builder.Register<IPoolService, PoolService>(Lifetime.Scoped);
        }
    }
}
