using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Infrastructure.DI
{
    public class MenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("fgerfargiej");
            builder.RegisterEntryPoint<MenuEntryPoint>(Lifetime.Scoped);
        }
    }
}
