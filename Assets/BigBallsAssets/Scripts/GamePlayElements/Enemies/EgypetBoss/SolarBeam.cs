using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SolarBeam : MonoBehaviour
{
    [SerializeField] private ParticleObject _laser;
    [SerializeField] private float _windupSize = 0.5f;
    [SerializeField] private float _activeSize = 3f;
    [SerializeField] private AttackMark _markPrefab;
    [SerializeField] private Collider _collider;

    private AttackMark _mark;
    private IDamageService _damageService;
    private IPoolService _poolService;
    private float _windup;
    private float _active;
    private float _damage;
    private LayerMask _mask;
    private bool _baked;
    private bool _fired;

    [Inject]
    public void Cosctruct (IDamageService damageService, IPoolService poolService)
    {
        _damageService = damageService;
        _poolService = poolService;
    }

    public void Init (float windup, float active, float damage, LayerMask mask)
    {
        _windup = windup;
        _active = active;
        _damage = damage;
        _mask = mask;
        _baked = false;
        _fired = false;

        Setup();
    }

    private void Update ()
    {
        if (_mark == null)
            return;

        if (_baked == false)
        {
            _windup -= Time.deltaTime;

            if (_windup <= 0f)
            {
                _baked = true;
                FireOnce();
            }
        }
        else if (_fired == false)
        {
            _active -= Time.deltaTime;

            if (_active <= 0f)
            {
                _fired = true;
                _collider.enabled = false;

                _poolService.Release(_mark);
                _poolService.Release(this);
                _mark = null;
            }
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _damageService.ApplyDamage(player, _damage);
        }
    }

    private void FireOnce ()
    {
        _laser.ParticleSystem.startSize = _activeSize;
        _collider.enabled = true;
    }

    private void Setup()
    {
        Vector3 markSpawnPoint = new Vector3(transform.position.x, 0, transform.position.z);
        _mark = _poolService.Get(_markPrefab, markSpawnPoint);
        _laser.Play();
        _laser.ParticleSystem.startSize = _windupSize;
    }
}