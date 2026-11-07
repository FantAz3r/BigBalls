using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunOrb : MonoBehaviour
{
    [SerializeField] private Transform _visual;

    private SunPharaohConfig _cfg;
    private SunPharaoh _owner;
    private IPlayerProvider _playerProvider;
    private ICoroutineRunner _coroutineRunner;
    private IDamageService _damageService;
    private IEntityRepository _entityRepository;
    private Coroutine _routine;
    private Vector3 _center;

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IPlayerProvider playerProvider, IDamageService damageService, IEntityRepository entityRepository)
    {
        _coroutineRunner = coroutineRunner;
        _playerProvider = playerProvider;
        _damageService = damageService;
        _entityRepository = entityRepository;
    }

    public void Init (SunPharaoh owner, SunPharaohConfig cfg, Vector3 center)
    {
        _owner = owner;
        _cfg = cfg;
        _center = center;
        transform.position = center;
        _routine = _coroutineRunner.StartCoroutine(Run());
    }

    private IEnumerator Run ()
    {
        float elapsed = 0f;
        float shootTimer = 0f;

        while (elapsed < _cfg.SunOrbDuration)
        {
            // Вращение
            if (_visual != null)
                _visual.Rotate(Vector3.up, _cfg.SunOrbRotationSpeed * Time.deltaTime, Space.Self);


            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shootTimer += _cfg.SunOrbBeamInterval;
                FireVolley();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void FireVolley ()
    {
        int n = Mathf.Max(1, _cfg.SunOrbBeamsPerVolley);
        for (int i = 0; i < n; i++)
        {
            float angle = (360f / n) * i + Time.time * _cfg.SunOrbRotationSpeed; // «равномерно, но с фазой»
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;

            if (Physics.Raycast(_center, dir, out var hit, _cfg.SunOrbBeamLength, _cfg.SunOrbBeamMask))
            {
                if (hit.collider.TryGetComponent(out Player player))
                {
                    StatHolder statHolder = _entityRepository.GetStatHolder(player);


                }
                    dmg.TakeDamage(_cfg.SunOrbBeamDamage);

                if (hit.collider.TryGetComponent(out IAccuracyDebuffable target))
                    target.ApplyAccuracyDebuff(_cfg.SunOrbAccuracyDebuff, _cfg.SunOrbAccuracyDebuffDuration);
            }

            // Визуал луча — на ваше усмотрение (LineRenderer/VFX)
        }
    }
}