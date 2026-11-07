using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.UI;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Ball : MonoBehaviour, IEntity
    {
        [SerializeField] private Collider _body;

        [field: SerializeField] public BallConfig Config { get; private set; }
        [field: SerializeField] public ParticleObject Particle { get; private set; }
        [field: SerializeField] public Collider TriggerCollider { get; private set; }

        private LayerMask _playerLayer;
        private LayerMask _enemyLayers;
        private List<ICollisionStrategy> _collisionStrategies;
        private IEffectFactory _effectFactory;
        private float _stackCount;
        private DamageCalculator _damageCalculator;

        public List<EffectBehaviour> EffectBehaviours { get; private set; }
        public HealthHandler<Ball> DeathHandler { get; private set; }
        public BallModel Model { get; private set; }
        public bool IsMaterial { get; private set; }
        public bool CanReturnToBag { get; private set; } = false;
        public IMover Mover { get; private set; }
        public int Id { get; private set; }
        public Transform Transform => transform;
        public EntityEventHandler EventHandler { get; private set; }
        public float AppliedDamage { get; private set; }
        public float TotalDamage { get; private set; }


        private void Awake ()
        {
            _enemyLayers = LayerMask.GetMask("Enemy", "Shield");
            _playerLayer = LayerMask.GetMask("Player");
            EventHandler = new EntityEventHandler();
            TotalDamage = 0;
        }

        private void OnCollisionEnter (Collision collision)
        {
            _stackCount = 0;

            foreach (var strategy in _collisionStrategies.ToList())
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

                foreach (var strategy in _collisionStrategies.ToList())
                {
                    if (strategy.HandleCollision(this, collision))
                        break;
                }
            }
        }

        private void OnTriggerEnter (Collider other)
        {
            if (IsMaterial)
                return;

            foreach (var strategy in _collisionStrategies.ToList())
            {
                strategy.HandleTrigger(this, other);
            }
        }

        private void OnEnable ()
        {
            IsMaterial = Config.IsMaterial;
            MakeGhost(IsMaterial);

            if (EffectBehaviours != null && EffectBehaviours.Count > 0)
            {
                foreach (var effect in EffectBehaviours)
                {
                    effect.Subscribe(this);
                }
            }
        }

        private void OnDisable ()
        {
            if (EffectBehaviours != null && EffectBehaviours.Count > 0)
            {
                foreach (var effect in EffectBehaviours)
                {
                    effect.Unsubscribe(this);
                }
            }
        }

        private void OnDestroy ()
        {
            UnsubscribeEffects();
        }

        [Inject]
        public void Construct (IEffectFactory ballEffectFactory, DamageCalculator damageCalculator)
        {
            _effectFactory = ballEffectFactory;
            _damageCalculator = damageCalculator;
            _damageCalculator.Add(this);
        }

        public void Init (
            int id,
            BallModel model,
            IMover mover,
            List<ICollisionStrategy> collisionStrategies,
            HealthHandler<Ball> deathHandler)
        {
            AppliedDamage = 0;
            DeathHandler = deathHandler;
            Id = id;
            Model = model;
            Mover = mover;
            _collisionStrategies = collisionStrategies;
            _body = GetComponent<Collider>();
            SetCanReturnToBag(false);
            EffectBehaviours = _effectFactory.Create(Config.Effects, Model.Level);
        }

        public void AddEffects (List<ArtefactModel> artefacts)
        {
            foreach (var artefact in artefacts)
            {
                EffectBehaviours.AddRange(_effectFactory.Create(artefact.ArtefactConfig.Effects, artefact.Level));
            }
        }

        public void MakeGhost (bool isMaterial)
        {
            IsMaterial = isMaterial;

            if (_body != null)
            {
                _body.excludeLayers = IsMaterial == false
                    ? _body.excludeLayers | _enemyLayers
                    : _body.excludeLayers & ~_enemyLayers;
            }
        }

        public void Subscribe ()
        {
            foreach (var effect in EffectBehaviours)
            {
                effect.Subscribe(this);
            }
        }

        public void UnsubscribeEffects ()
        {
            CanReturnToBag = false;

            foreach (var effect in EffectBehaviours)
            {
                effect.Unsubscribe(this);
            }

            EffectBehaviours.Clear();
        }

        public void SetCanReturnToBag (bool canReturnToBag)
        {
            CanReturnToBag = canReturnToBag;

            if(Config.IsPlayerBall)
                _body.excludeLayers = canReturnToBag ? _body.excludeLayers & ~_playerLayer : _body.excludeLayers | _playerLayer;
        }

        public void SetIsMaterial (bool isMaterial) => IsMaterial = isMaterial;

        public void ClearCollisionStrategies ()
        {
            _collisionStrategies.Clear();
        }

        public void CalculateDamage (float damage)
        {
            AppliedDamage += damage;
            TotalDamage += damage;
        }
    }
}