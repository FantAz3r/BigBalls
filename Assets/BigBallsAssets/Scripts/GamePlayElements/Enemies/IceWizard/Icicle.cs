using System;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class Icicle : MonoBehaviour
{
    [SerializeField] private Transform _visual;
    [SerializeField] private ParticleObject _particle;
    [SerializeField] private AttackMark _markPrefab;

    private AttackMark _mark;
    private IDamageService _damageService;
    private IPoolService _poolService;
    private float _fallTime;
    private float _telegraphTime;
    private float _damage;
    private LayerMask _mask;
    private Vector3 _target;
    private Vector3 _spawnTop;
    private float _t;
    private bool _impacted;
    private Action _onHit;

    [Inject]
    public void Construct (IDamageService damageService, IPoolService poolService)
    {
        _damageService = damageService;
        _poolService = poolService;
    }

    public void Init (Vector3 target, float fallHeight, float telegraphTime, float fallTime, float damage, LayerMask mask, Action onHit)
    {
        _target = target;
        _telegraphTime = telegraphTime;
        _fallTime = fallTime;
        _damage = damage;
        _mask = mask;
        _spawnTop = target + Vector3.up * fallHeight;
        transform.position = _spawnTop;
        _onHit = onHit;

        _t = 0f;
        _impacted = false;
        _mark = _poolService.Get(_markPrefab);
        _mark.transform.position = new Vector3(target.x, 0, target.z);
    }

    private void Update ()
    {
        _t += Time.deltaTime;

        if (_t < _telegraphTime)
        {
            return;
        }

        float fallT = (_t - _telegraphTime) / _fallTime;

        if (fallT >= 1f && _impacted == false)
        {
            _impacted = true;
            Impact();
            return;
        }

        if (fallT < 1f)
            transform.position = Vector3.Lerp(_spawnTop, _target, Mathf.Clamp01(fallT));
    }

    private void Impact ()
    {
        var hits = Physics.OverlapSphere(_target, 0.5f, _mask);

        for (int i = 0; i < hits.Length; i++)
            if (hits[i].TryGetComponent(out Player player))
                _damageService.ApplyDamage(player, _damage);

        ParticleObject particleObject = _poolService.Get(_particle, transform.position, Quaternion.identity);
        particleObject.Play();
        ReleaseMark();
        _poolService.Release(this);
        _onHit.Invoke();
    }

    private void ReleaseMark ()
    {
        if (_mark != null)
        {
            _poolService.Release(_mark);
            _mark = null;
        }
    }
}