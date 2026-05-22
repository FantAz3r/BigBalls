using VContainer;

namespace BigBalls.Infrastructure.DI
{
    public class ObjectResolverProvider : IObjectResolverProvider
    {
        private IObjectResolver _currentResolver;

        public IObjectResolver CurrentResolver => _currentResolver;

        public void UpdateResolver(IObjectResolver newResolver)
        {
            _currentResolver = newResolver;
        }
    }
}