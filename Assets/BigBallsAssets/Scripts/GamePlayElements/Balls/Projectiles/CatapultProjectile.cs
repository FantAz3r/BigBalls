using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using System;
using UnityEngine;
using VContainer;


public class CatapultProjectile : MonoBehaviour
{
    [SerializeField] private ParticleObject _particle;

    private float _damageRange;
    private float _damage;
    private IDamageService _damageService;
    private IParticleFactory _particleFactory;

    public event Action<CatapultProjectile> OnProjectileReturned;
    [field: SerializeField] public Collider Collider { get; private set; }

    public void Initialize(float damage, float damageRange)
    {
        _damage = damage;
        _damageRange = damageRange;
        Collider.enabled = false;
    }

    [Inject]
    public void Construct(IDamageService damageService, IParticleFactory particleFactory)
    {
        _damageService = damageService;
        _particleFactory = particleFactory;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _damageRange);

        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent(out IEntity entity))
            {
                _damageService.ApplyDamage(entity, _damage);
            }
        }

        _particleFactory.Create(_particle, transform.position);
        OnProjectileReturned?.Invoke(this);
    }
}


