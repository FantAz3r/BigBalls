using VContainer;
using BigBalls.Infrastructure.DI;
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