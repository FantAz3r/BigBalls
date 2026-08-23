using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.StaticData;
using BigBalls.UI;
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
            { typeof(WindowData), "Data/WindowData"},
            { typeof(LevelData), "Data/LevelData"},
            { typeof(ChestData), "Data/ChestData"},
            { typeof(EnemyData), "Data/EnemyData"},
            { typeof(CardsData), "Data/CardsData"},
            { typeof(ParticleData), "Data/Particle Data"},
            { typeof(LootData), "Data/LootData"},

            { typeof(Player), "Player/Player"},
            { typeof(UIRoot), "UI/UIRoot"},
            { typeof(PlayerConfig), "Configs/PlayerConfig"},
            { typeof(Camera), "Prefabs/Main Camera"},
            { typeof(ObjectContainer), "Prefabs/GameObjectContainer"},
            //{ typeof(BallBehaivourData), "Data/BallBehaivourData"},
            {typeof(PoolServiceConfig), "Configs/PoolServiceConfig"},
            {typeof(CardView), "UI/CardView"},
            {typeof(DamageText), "Prefabs/DamageTextObject"},
            {typeof(StatTextHolder), "UI/StatTextHolder"},
            {typeof(StatsSlot), "UI/StatSlot"},
            {typeof(LightningBolt), "LightningBolt"},

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

            if ((T)@object == null)
            {
                throw new ArgumentNullException(nameof(T));
            }

            return (T) @object;
        }
    }
}
