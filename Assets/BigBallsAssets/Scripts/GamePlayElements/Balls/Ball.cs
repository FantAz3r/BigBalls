using BigBalls.Configs;
using BigBalls.Factories;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Ball : MonoBehaviour, IEntity
    {
        private List<ICollisionStrategy> _collisionStrategies;
        private IEffectFactory _effectFactory;
        private float _stackCount;

        [field: SerializeField] public BallConfig Config { get; private set; }
        [field: SerializeField] public ParticleObject Particle { get; private set; }
        public List<EffectBehaviour> EffectBehaviours { get; private set; }
        public DeathHandler<Ball> DeathHandler { get; private set; }
        public BallModel Model { get; private set; }
        public bool IsMaterial { get; private set; }
        public bool CanReturnToBag { get; private set; } = false;
        public IMover Mover { get; private set; }
        public int Id { get; private set; }
        public Transform Transform => transform;
        public EntityEventHandler EventHandler { get; private set; }
        public float AppliedDamage { get; private set; }

        private void Awake()
        {
            EventHandler = new EntityEventHandler();
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

        private void OnCollisionStay(Collision collision)
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

        private void OnDestroy()
        {
            UnsubscribeEffects();
        }

        public void Construct(
            int id,
            BallModel model,
            IMover mover,
            List<ICollisionStrategy> collisionStrategies,
            DeathHandler<Ball> deathHandler,
            IEffectFactory ballEffectFactory)
        {
            AppliedDamage = 0;
            DeathHandler = deathHandler;
            Id = id;
            Model = model;
            Mover = mover;
            _collisionStrategies = collisionStrategies;
            _effectFactory = ballEffectFactory;

            EffectBehaviours = _effectFactory.Create(Config.Effects, Model.Level);
        }

        public void AddEffects(List<ArtefactModel> artefacts)
        {
            foreach (var artefact in artefacts)
            {
                EffectBehaviours.AddRange(_effectFactory.Create(artefact.ArtefactConfig.Effects, artefact.Level));
            }
        }

        public void Subscribe()
        {
            foreach (var effect in EffectBehaviours)
            {
                effect.Subscribe(this);
            }
        }

        public void UnsubscribeEffects()
        {
            CanReturnToBag = false;

            foreach (var effect in EffectBehaviours)
            {
                effect.Unsubscribe(this);
            }

            EffectBehaviours.Clear();
        }

        public void SetCanReturnToBag(bool canReturnToBag) => CanReturnToBag = canReturnToBag;
        public void SetIsMaterial(bool isMaterial) => IsMaterial = isMaterial;

        public void ClearCollisionStrategies()
        {
            _collisionStrategies.Clear();
        }

        public void AddDamage(float damage)
        {
            AppliedDamage += damage;
        }
    }
}