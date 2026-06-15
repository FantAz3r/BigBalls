using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Infrastructure.DI;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public class EffectFactory : IEffectFactory
    {
        private readonly IObjectResolverProvider _objectResolverProvider;

        private Dictionary<BehaviourType, Func<EffectConfig, int, EffectBehaviour>> _effectToFactory = new();

        public EffectFactory(IObjectResolverProvider objectResolverProvider)
        {
            _objectResolverProvider = objectResolverProvider;

            _effectToFactory = new Dictionary<BehaviourType, Func<EffectConfig, int, EffectBehaviour>>()
            {
                [BehaviourType.Damage] = CreateDamageEffect,
                [BehaviourType.Burn] = CreateFireEffect,
                [BehaviourType.Freeze] = CreateIceEffect,
                //[BehaviourType.Tunder] = CreateTunderEffect,
            };
        }

        public List<EffectBehaviour> Create(List<EffectConfig> effectConfigs, int level)
        {
            List<EffectBehaviour> effectBehaviours = new List<EffectBehaviour>();

            foreach (var effectConfig in effectConfigs)
            {
                EffectBehaviour effectBehaviour = _effectToFactory[effectConfig.Type](effectConfig, level);
                _objectResolverProvider.CurrentResolver.Inject(effectBehaviour);
                effectBehaviours.Add(effectBehaviour);
            }

            return effectBehaviours;
        }

        private EffectBehaviour CreateDamageEffect(EffectConfig config, int level) => new Damage(config as DamageConfig, level);
        private EffectBehaviour CreateFireEffect(EffectConfig config, int level) => new Fire(config as FireConfig, level);
        private EffectBehaviour CreateIceEffect(EffectConfig config, int level) => new Ice(config as IceConfig, level);
        //private EffectBehaviour CreateTunderEffect(EffectConfig config) => new Tunder(config as TunderConfig);
    }
}
