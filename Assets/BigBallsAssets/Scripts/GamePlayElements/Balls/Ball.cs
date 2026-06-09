using BigBalls.Factories;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Ball : MonoBehaviour, IEntity
    {
        private List<ICollisionStrategy> _collisionStrategies;
        private List<EffectBehaviour> _effectBehaviours;
        private IBallEffectFactory _ballEffectFactory;

        public event Action<int> Hited;
        public event Action<Ball> Disabled;
        public event Action<Ball> Returned;

        [field: SerializeField] public BallConfig Config { get; private set; }
        public DeathHandler<Ball> DeathHandler { get; private set; }
        public bool IsMaterial { get; private set; }
        public bool CanReturnToBag { get; private set; } = false;
        public Mover Mover { get; private set; }
        public int Id { get; private set; }
        public Transform Transform => transform;


        private void OnCollisionEnter(Collision collision)
        {
            foreach (var strategy in _collisionStrategies)
            {
                if (strategy.HandleCollision(this, collision))
                    break;
            }
        }
        
        private void OnDestroy()
        {
            UnsubscribeEffects();
        }

        public void UnsubscribeEffects()
        {
            foreach (var effect in _ballEffectFactory.Create(Config))
            {
                effect?.Unsubscribe(this);
            }
        }

        public void Construct(int id, Mover mover, List<ICollisionStrategy> collisionStrategies, DeathHandler<Ball> deathHandler, IBallEffectFactory ballEffectFactory)
        {
            DeathHandler = deathHandler;
            _collisionStrategies = collisionStrategies;
            Id = id;
            Mover = mover;
            _ballEffectFactory = ballEffectFactory;
            IsMaterial = Config.IsMaterial;
            transform.localScale = new Vector3(Config.Radius, Config.Radius, Config.Radius);

            foreach (var effect in _ballEffectFactory.Create(Config))
            {
                effect.Subscribe(this);
            }
        }

        public void OnHit(int id) => Hited?.Invoke(id);
        
        public void OnReturn(Ball ball)
        {
            Returned?.Invoke(ball);
            Returned = null;
        }

        public void SetCanReturnToBag(bool canReturnToBag) => CanReturnToBag = canReturnToBag;
        public void SetIsMaterial(bool isMaterial) => IsMaterial = isMaterial;

        public void ReflectBall(Collision collision)
        {
            Vector3 currentDirection = new Vector3(Mover.Direction.normalized.x, 0, Mover.Direction.normalized.y);
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(currentDirection, normal);
            Mover.SetDirection(new Vector2(reflectedDirection.x, reflectedDirection.z));
        }
    }
}
