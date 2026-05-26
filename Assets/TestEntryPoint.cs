using BigBalls.Infrastructure.DI;
using VContainer;
using VContainer.Unity;

public class TestEntryPoint : IStartable
{
    public TestEntryPoint(IObjectResolverProvider objectResolverProvider, IObjectResolver objectResolver)
    {
        objectResolverProvider.UpdateResolver(objectResolver);

    }

    public void Start()
    {
    }
}