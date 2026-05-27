using BigBalls.Infrastructure.DI;
using VContainer;
using VContainer.Unity;

namespace BigBalls.Infrastructure
{
    public class LevelEntryPoint : IStartable
    {
        public LevelEntryPoint(IObjectResolverProvider objectResolverProvider, IObjectResolver objectResolver)
        {
            objectResolverProvider.UpdateResolver(objectResolver);
        }

        public void Start()
        {
        }
    }
}
