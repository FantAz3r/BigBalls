using BigBalls.Infrastructure;
using VContainer;
using VContainer.Unity;

public class TestLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<TestEntryPoint>();
        builder.Register<CreateLevelState>(Lifetime.Scoped);

    }
}
