using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Infrastructure
{
    public class MenuEntryPoint: IStartable
    {
        public MenuEntryPoint(IObjectResolverProvider objectResolverProvider, IObjectResolver objectResolver)
        {
            objectResolverProvider.UpdateResolver(objectResolver);

        }

        public void Start()
        {
        }
    }
}