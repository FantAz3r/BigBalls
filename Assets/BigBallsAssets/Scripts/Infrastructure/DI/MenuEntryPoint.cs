using VContainer;
using VContainer.Unity;
using BigBalls.Infrastructure.DI;

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