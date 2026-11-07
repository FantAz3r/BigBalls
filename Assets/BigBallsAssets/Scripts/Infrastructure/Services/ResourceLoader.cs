using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.StaticData;
using BigBalls.UI;
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
            { typeof(SoundData), "Data/SoundData"},

            { typeof(Player), "Player/Player"},
            { typeof(PoolServiceConfig), "Configs/PoolServiceConfig"},
            { typeof(PlayerConfig), "Configs/PlayerConfig"},
            {typeof(LightningBolt), "LightningBolt"},

            { typeof(SoundObject), "Prefabs/SoundObject"},
            { typeof(DamageText), "Prefabs/DamageTextObject"},
            { typeof(Camera), "Prefabs/Main Camera"},
            { typeof(ObjectContainer), "Prefabs/GameObjectContainer"},

            { typeof(UIRoot), "UI/UIRoot"},
            { typeof(CardView), "UI/CardView"},
            { typeof(StatTextHolder), "UI/StatTextHolder"},
            { typeof(StatsSlot), "UI/StatSlot"},
            { typeof(SplitingCard), "UI/MergeSlot"},
            { typeof(GameConfig),  "Configs/GameConfig" }
        };

        public T Load<T> () where T : Object
        {
            string path = _staticObjects[typeof(T)];
            Type type = typeof(T);

            if (_cashedObjects.TryGetValue(type, out Object @object) == false)
            {
                @object = Resources.Load<T>(path);
                _cashedObjects.Add(type, @object);
            }

            if ((T) @object == null)
            {
                throw new ArgumentNullException(nameof(T));
            }

            return (T) @object;
        }
    }
}
