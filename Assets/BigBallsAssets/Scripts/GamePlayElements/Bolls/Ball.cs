using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private List<Behaviour> _effects = new();
    private Collider _collider;
    private bool _isMaterial;
    private float _size;

    public event Action<int> Hited;
    public float Speed { get; private set; }
    public float Damage { get; private set; }

    public void Construct(BallConfig ballConfig, IBallBehaivorFactory behaivorFactory)
    {
        Damage = ballConfig.Damage;
        Speed = ballConfig.Speed;
        _size = ballConfig.Radius;

        foreach (var behaviour in ballConfig.BehaviourTypes)
        {
            _effects.Add(behaivorFactory.Get(behaviour));
        }

        foreach (var effect in _effects)
        {
            effect.Init(this);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Physics.SphereCast(transform.position, _size, new Vector3(), out var hit);

        if (_isMaterial == false)
        {
            if (other.TryGetComponent(out IEntity entity))
            {
                Hited?.Invoke(entity.Id);
            }
        }
    }

    public void Set(bool isMaterial)
    {
        _collider.isTrigger = isMaterial == false;
        _isMaterial = isMaterial;
    }
}
