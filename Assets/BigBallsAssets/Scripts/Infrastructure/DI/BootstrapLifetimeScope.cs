using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BootstrapLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log("1");

        Debug.Log("2");
    }
}
