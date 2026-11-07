using System;
using UnityEngine;
[RequireComponent (typeof(SphereCollider))]
public class AuraTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _ball;

    private SphereCollider _sphereCollider;

    private Action<Collider> _onEnter;

    private void Awake ()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
    }

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