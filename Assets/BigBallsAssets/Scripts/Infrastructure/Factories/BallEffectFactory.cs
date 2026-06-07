using BigBalls.Factories;
using BigBalls.Infrastructure.DI;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public class BallEffectFactory : IBallEffectFactory
    {
        private readonly IObjectResolverProvider _objectResolverProvider;

        private Dictionary<BehaviourType, Func<EffectConfig, EffectBehaviour>> _effectToFactory = new();

        public BallEffectFactory(IObjectResolverProvider objectResolverProvider)
        {
            _objectResolverProvider = objectResolverProvider;

            _effectToFactory = new Dictionary<BehaviourType, Func<EffectConfig, EffectBehaviour>>()
            {
                [BehaviourType.Damage] = CreateDamageEffect,
                [BehaviourType.Burn] = CreateFireEffect,
                [BehaviourType.Freeze] = CreateIceEffect,
                //[BehaviourType.Tunder] = CreateTunderEffect,
            };
        }

        public List<EffectBehaviour> Create(BallConfig ballConfig)
        {
            List<EffectBehaviour> effectBehaviours = new List<EffectBehaviour>();

            foreach (var effectConfig in ballConfig.EffectConfigs)
            {
                EffectBehaviour effectBehaviour = _effectToFactory[effectConfig.Type](effectConfig);
                _objectResolverProvider.CurrentResolver.Inject(effectBehaviour);
                effectBehaviours.Add(effectBehaviour);
            }

            return effectBehaviours;
        }

        private EffectBehaviour CreateDamageEffect(EffectConfig config) => new Damage(config as DamageBehaviour);
        private EffectBehaviour CreateFireEffect(EffectConfig config) => new Fire(config as FireConfig);
        private EffectBehaviour CreateIceEffect(EffectConfig config) => new Ice(config as IceConfig);
        //private EffectBehaviour CreateTunderEffect(EffectConfig config) => new Tunder(config as TunderConfig);
    }
}
