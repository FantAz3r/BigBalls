using BigBalls.Configs;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigBalls.Services
{
    public class ResourceLoader : IResourceLoader
    {
        private Dictionary<Type, Object> _cashedObjects = new Dictionary<Type, Object>();

        private Dictionary<Type, string> _staticObjects = new()
        {
            { typeof(WindowData), "Data/WindowData"}
        };

        public T Load<T>() where T : Object
        {
            string path = _staticObjects[typeof(T)];
            Type type = typeof(T);

            if (_cashedObjects.TryGetValue(type, out Object @object) == false)
            {
                @object = Resources.Load<T>(path);
                _cashedObjects.Add(type, @object);
            }

            return (T)@object;
        }
    }
}
