using UnityEngine;

namespace BigBalls.Services
{
    public interface IResourceLoader
    {
        T Load<T>() where T : Object;
    }
}