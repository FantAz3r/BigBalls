using System;
using UnityEngine;

public class LavaTrigger : MonoBehaviour
{
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _ball;
    private Action<Collider> _onEnter;

    public void Initialize (Action<Collider> onEnter, float radius)
    {
        _onEnter = onEnter;
        _sphereCollider.radius = radius;

        if (_particleSystem != null)
        {
            var shape = _particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.radius = radius * _ball.localScale.x;
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        _onEnter?.Invoke(other);
    }
}