using BigBalls.Configs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Factories
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] private FactoryConfig _config;

        private Dictionary<Type, MonoBehaviour> _allPrefabs;

        private void Awake()
        {
            InitFactory(_config);
        }

        public void InitFactory(FactoryConfig factoryConfig)
        {
            _allPrefabs = new Dictionary<Type, MonoBehaviour>();

            foreach (var prefab in factoryConfig.GlobalPrefabConfig.Prefabs)
            {
                Debug.Log(prefab.GetType());
                _allPrefabs[prefab.GetType()] = prefab;
            }
        }

        public MonoBehaviour Create(Type type)
        {
            Debug.Log(type);

            if (!_allPrefabs.ContainsKey(type))
                return null;

            MonoBehaviour prefab = _allPrefabs[type];

            if (prefab == null)
                return null;

            return GameObject.Instantiate(prefab);
        }
    }
}