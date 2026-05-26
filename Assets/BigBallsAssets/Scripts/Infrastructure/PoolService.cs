using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Infrastructure.DI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BigBalls.Services
{
    public class PoolService : IPoolService
    {
        [SerializeField] private PoolManagerConfig _config;

        private Dictionary<Type, Stack<MonoBehaviour>> _objectPools;

        [SerializeField] private Factory _factory;
        private readonly ObjectContainer _objectContainer;

        public PoolService(ObjectContainer  objectContainer)
        {
            _objectContainer = objectContainer;
        }

        private void Start()
        {
            FillPool(_config);
        }

        public void FillPool(PoolManagerConfig config)
        {
            _objectPools = new Dictionary<Type, Stack<MonoBehaviour>>(config.Objects.Count);

            foreach (var prefabInfo in config.Objects)
            {
                if (prefabInfo.Prefab == null)
                    continue;

                Stack<MonoBehaviour> newQueue = new();
                Type type = prefabInfo.Prefab.GetType();
                _objectPools[type] = newQueue;

                for (int i = 0; i < prefabInfo.StartPoolCount; i++)
                {
                    MonoBehaviour newObject = _factory.Create(type);
                    newObject.gameObject.SetActive(false);
                    newObject.transform.parent = _objectContainer.transform;
                    newQueue.Push(newObject);
                }
            }
        }

        public void ClearPool()
        {
            foreach (var pool in _objectPools)
                foreach (var obj in pool.Value)
                    Object.Destroy(obj.gameObject);
        }
    }
}