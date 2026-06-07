using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Infrastructure.DI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BigBalls.Services
{
    public class PoolService : IPoolService
    {
        [SerializeField] private PoolManagerConfig _config;

        //private Dictionary<FactoryType, Dictionary<Type, ObjectPool<T>>> _objectPools;

        [SerializeField] private Factory _factory;
        private readonly ObjectContainer _objectContainer;

        public PoolService(ObjectContainer  objectContainer)
        {
            _objectContainer = objectContainer;
        }

       
    }
}