using BigBalls.Configs;
using BigBalls.Factories;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Ball : MonoBehaviour, IEntity
    {
        private List<ICollisionStrategy> _collisionStrategies;
        private List<EffectBehaviour> _effectBehaviours;
        private IEffectFactory _ballEffectFactory;
        private float _stackCount;

        [field: SerializeField] public BallConfig Config { get; private set; }
        public DeathHandler<Ball> DeathHandler { get; private set; }
        public bool IsMaterial { get; private set; }
        public bool CanReturnToBag { get; private set; } = false;
        public IMover Mover { get; private set; }
        public int Id { get; private set; }
        public int Level { get; private set; } = 1;
        public Transform Transform => transform;
        public EntityEventHandler EventHandler { get; private set; }

        public float AppliedDamage { get; private set; }

        private void Awake()
        {
            EventHandler = new EntityEventHandler();
            transform.localScale = new Vector3(Config.Radius, Config.Radius, Config.Radius);
            IsMaterial = Config.IsMaterial;

        }


        private void OnCollisionEnter(Collision collision)
        {
            _stackCount = 0;

            foreach (var strategy in _collisionStrategies)
            {
                if (strategy.HandleCollision(this, collision))
                    break;
            }
        }

        private void OnCollisionStay (Collision collision)
        {
            _stackCount += Time.deltaTime;

            if (_stackCount >= 0.1f)
            {
                _stackCount = 0;

                foreach (var strategy in _collisionStrategies)
                {
                    if (strategy.HandleCollision(this, collision))
                        break;
                }
            }
        }

        private void OnDestroy ()
        {
            UnsubscribeEffects();
        }

        public void Construct(
            int id,
            int level,
            IMover mover,
            List<ICollisionStrategy> collisionStrategies,
            DeathHandler<Ball> deathHandler,
            IEffectFactory ballEffectFactory)
        {
            AppliedDamage = 0;
            DeathHandler = deathHandler;
            Id = id;
            Level = level;
            Mover = mover;
            _collisionStrategies = collisionStrategies;
            _ballEffectFactory = ballEffectFactory;

            _effectBehaviours = _ballEffectFactory.Create(Config.Effects, Level);

            foreach (var effect in _effectBehaviours)
            {
                effect.Subscribe(this);
            }
        }

        public void AddEffects(List<EffectBehaviour> effects)
        {
            foreach (var effect in effects)
                effect.Subscribe(this);

            _effectBehaviours.AddRange(effects);
        }

        public void UnsubscribeEffects()
        {
            CanReturnToBag = false;

            foreach (var effect in _effectBehaviours)
            {
                effect?.Unsubscribe(this);
            }

            _effectBehaviours.Clear();
        }

        public void SetCanReturnToBag(bool canReturnToBag) => CanReturnToBag = canReturnToBag;
        public void SetIsMaterial(bool isMaterial) => IsMaterial = isMaterial;

        public void AddDamage(float damage)
        {
            AppliedDamage += damage;
        }
    }
}