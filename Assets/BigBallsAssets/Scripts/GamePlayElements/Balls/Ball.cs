using BigBalls.Factories;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Ball : MonoBehaviour
    {
        private List<EffectBehaviour> _effects = new();
        private Collider _collider;
        private bool _isMaterial;
        public event Action<int> Hited;

        public BallConfig Config { get; private set; }

        public void Construct(BallConfig ballConfig, IBallBehaivorFactory behaivorFactory)
        {
            _isMaterial = ballConfig.IsMaterial;
            _effects = behaivorFactory.Create(Config);

            foreach (var effect in _effects)
            {
                effect.Init(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            other.ClosestPoint(transform.position);
            Physics.SphereCast(transform.position, Config.Radius, new Vector3(), out var hit);

            if (_isMaterial == false)
            {
                //логика Reflect;
            }

            if (other.TryGetComponent(out IEntity entity))
            {
                Hited?.Invoke(entity.Id);
            }
        }

        public void Set(bool isMaterial)
        {
            _isMaterial = isMaterial;
            _collider.isTrigger = _isMaterial == false;
        }
    }
}
