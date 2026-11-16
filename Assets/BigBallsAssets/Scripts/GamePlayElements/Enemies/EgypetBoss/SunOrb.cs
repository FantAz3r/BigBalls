using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunOrb : MonoBehaviour, IEntity
{
    [SerializeField] private Transform _visual;
    [SerializeField] private AttackMark _beamPreviewPrefab;
    [SerializeField] private List<ParticleObject> _sunBeams;

    private SunPharaohConfig _cfg;
    private ICoroutineRunner _coroutineRunner;
    private IDamageService _damageService;
    private IPoolService _poolService;
    private DebuffService _debuffService;
    private StatHolder _statHolder;
    private Stat _rotationSpeed;
    private Coroutine _coroutine;
    private bool _canShoot;

    private readonly List<AttackMark> _previews = new List<AttackMark>();
    private float _currentAngle;

    public int Id => 0;
    public Transform Transform => transform;
    public EntityEventHandler EventHandler { get; private set; }
    public HealthHandler<SunOrb> HealthHandler { get; private set; }
    public Collider TriggerCollider => throw new System.NotImplementedException();

    private void OnDisable ()
    {
        _canShoot = false;

        if (_coroutineRunner != null && _coroutine != null)
        {
            _coroutineRunner.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        HidePreviews();
    }

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IDamageService damageService, DebuffService debuffService, IPoolService poolService)
    {
        _coroutineRunner = coroutineRunner;
        _damageService = damageService;
        _debuffService = debuffService;
        _poolService = poolService;
    }

    public void Init (StatHolder statHolder, SunPharaohConfig cfg, Vector3 center, HealthHandler<SunOrb> healthHandler)
    {
        HealthHandler = healthHandler;
        EventHandler = new();
        _cfg = cfg;
        transform.position = center;
        _statHolder = statHolder;
        _statHolder.TryGetStat(out Stat rotationSpeed, StatType.RotationSpeed);
        _rotationSpeed = rotationSpeed;

        TryStart();
    }

    private void TryStart ()
    {
        if (_canShoot || _cfg == null || _coroutineRunner == null) return;

        _canShoot = true;
        _coroutine = _coroutineRunner.StartCoroutine(Run());
    }

    private IEnumerator Run ()
    {
        while (_canShoot)
        {
            float rotateTime = _cfg.SunOrbRotateDuration;
            float t = 0f;

            while (t < rotateTime && _canShoot)
            {
                float dt = Time.deltaTime;
                _currentAngle = Mathf.Repeat(_currentAngle + _rotationSpeed.CurrentValue * dt, 360f);

                ApplyRotationToVisual();

                t += dt;
                yield return null;
            }

            ShowPreviews();

            float prepTime = _cfg.SunOrbPrepareDuration;
            t = 0f;

            while (t < prepTime && _canShoot)
            {
                float dt = Time.deltaTime;
                float slowdown = 1f - Mathf.Clamp01(t / prepTime);
                _currentAngle = Mathf.Repeat(_currentAngle + _rotationSpeed.CurrentValue * dt * slowdown, 360f);

                ApplyRotationToVisual();
                UpdatePreviews();

                t += dt;
                yield return null;
            }

            FireVolley();
            HidePreviews();

            float cooldownTime = _cfg.SunOrbBeamInterval;
            t = 0f;

            while (t < cooldownTime && _canShoot)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        HidePreviews();
    }

    private void ApplyRotationToVisual ()
    {
        if (_visual != null)
            _visual.rotation = Quaternion.AngleAxis(_currentAngle, Vector3.up);
    }

    private void ShowPreviews ()
    {
        int n = Mathf.Max(1, _cfg.SunOrbBeamsPerVolley);

        while (_previews.Count < n)
        {
            AttackMark mark = _poolService.Get(
                _beamPreviewPrefab,
                new Vector3(transform.position.x, 0, transform.position.z));
            _previews.Add(mark);
        }

        for (int i = 0; i < _previews.Count; i++)
        {
            if (_previews[i] != null)
            {
                _previews[i].enabled = i < n;
            }
        }

        UpdatePreviews();
    }

    private void UpdatePreviews ()
    {
        int n = Mathf.Max(1, _cfg.SunOrbBeamsPerVolley);

        for (int i = 0; i < n && i < _previews.Count; i++)
        {
            AttackMark mark = _previews[i];
            if (mark == null) continue;

            Vector3 dir = GetBeamDirection(i);
            mark.transform.position = transform.position + dir * _cfg.SunOrbPreviewOffset;
            mark.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }

    private void HidePreviews ()
    {
        for (int i = 0; i < _previews.Count; i++)
        {
            if (_previews[i] == null) continue;
            _poolService.Release(_previews[i]);
            _previews[i] = null;
        }

        _previews.Clear();
    }

    private Vector3 GetBeamDirection (int index)
    {
        int beamsCount = Mathf.Max(1, _cfg.SunOrbBeamsPerVolley);
        float angle = (360f / beamsCount) * index + _currentAngle;
        return Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
    }

    private void FireVolley ()
    {
        int beamsCount = Mathf.Max(1, _cfg.SunOrbBeamsPerVolley);
        int available = Mathf.Min(beamsCount, _sunBeams.Count);
        Vector3 origin = transform.position;

        for (int i = 0; i < beamsCount; i++)
        {
            Vector3 dir = GetBeamDirection(i);

            if (i < available)
                _sunBeams[i].Play();

            if (Physics.Raycast(origin, dir, out var hit, _cfg.SunOrbBeamLength, _cfg.SunOrbBeamMask))
            {
                if (hit.collider.TryGetComponent(out Player player))
                {
                    _damageService.ApplyDamage(player, _cfg.SunOrbBeamDamage);
                    _debuffService.Apply(
                        EffectType.Blind,
                        player,
                        _cfg.SunOrbAccuracyDebuffDuration,
                        _cfg.SunOrbAccuracyDebuff);
                }
                else if (hit.collider.TryGetComponent(out SunPharaoh owner))
                {
                    _damageService.ApplyHeal(owner, _cfg.SunOrbBeamDamage, Color.green);
                }
            }
        }
    }
}