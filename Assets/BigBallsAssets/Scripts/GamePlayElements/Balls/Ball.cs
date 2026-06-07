using BigBalls.Factories;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Ball : MonoBehaviour, IEntity
    {
        private List<EffectBehaviour> _effects = new();
        private Collider _collider;
        private DeathHandler<Ball> _ballDeathHandler;
        private bool _isMaterial;
        private bool _canReturnToBag = false;

        public event Action<int> Hited;
        public event Action<Ball> Disabled;
        public event Action<Ball> Returned;

        [field: SerializeField] public BallConfig Config { get; private set; }
        public Mover Mover { get; private set; }
        public int Id { get; private set; }
        public Transform Transform => transform;


        private void OnEnable() => _ballDeathHandler?.Subscribe();

        private void OnDisable()
        {
            _ballDeathHandler?.Unsubscribe();
            Disabled?.Invoke(this);
        }

        public void Construct(IBallBehaivorFactory behaivorFactory, int id, Mover mover, DeathHandler<Ball> ballDeathHandler)
        {
            Id = id;
            Mover = mover;
            _ballDeathHandler = ballDeathHandler;
            _isMaterial = Config.IsMaterial;
            _effects = behaivorFactory.Create(Config);
            transform.localScale = new Vector3(Config.Radius, Config.Radius, Config.Radius);

            foreach (var effect in _effects)
            {
                effect.Init(this);
            }

            _ballDeathHandler.Subscribe();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<Player>(out _))
            {
                if (_canReturnToBag)
                {
                    Returned?.Invoke(this);
                }

                return;
            }

            if (_isMaterial)
            {
                ReflectBall(collision);
            }

            if (collision.gameObject.TryGetComponent<BackWall>(out _))
            {
                _canReturnToBag = true;

                
            }

            if (collision.gameObject.TryGetComponent(out IEntity entity))
            {
                Hited?.Invoke(entity.Id);
            }
        }

        public void Set(bool isMaterial)
        {
            _isMaterial = isMaterial;
            _collider.isTrigger = _isMaterial == false;
        }

        private void ReflectBall(Collision collision)
        {
            Vector3 currentDirection = new Vector3(Mover.Direction.normalized.x, 0, Mover.Direction.normalized.y);
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(currentDirection, normal);
            Mover.SetDirection(new Vector2(reflectedDirection.x, reflectedDirection.z));
        }
    }
}
