using VContainer;

namespace BigBalls.Infrastructure.DI
{
    public interface IObjectResolverProvider
    {
        IObjectResolver CurrentResolver { get; }
        void UpdateResolver(IObjectResolver newResolver);
    }
}