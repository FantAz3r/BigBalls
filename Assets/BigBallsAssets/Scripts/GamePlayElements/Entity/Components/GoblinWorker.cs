using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class GoblinWorker : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private SoundStruct _hitSound;

    private IPlayerProvider _playerProvider;
    private IDamageService _damageService;
    private ICoroutineRunner _coroutineRunner;
    private GoblinsShip _ship;
    private Coroutine _coroutine;
    private Stat _attackSpeed;
    private SoundSpawner _soundSpawner;
    private bool _canRepair = true;

    [Inject]
    public void Construct (IDamageService damageService, IPlayerProvider playerProvider, ICoroutineRunner coroutineRunner, SoundSpawner soundSpawner)
    {
        _coroutineRunner = coroutineRunner;
        _damageService = damageService;
        _playerProvider = playerProvider;
        _soundSpawner = soundSpawner;
    }

    private void Start ()
    {
        _attackSpeed = _enemy.StatHolder[StatType.AttackSpeed];
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.TryGetComponent(out GoblinsShip goblinsShip))
        {
            _ship = goblinsShip;
            _enemy.Rotator.SetTarget(goblinsShip.transform);
            _coroutine = _coroutineRunner.StartCoroutine(WorkRoutine());
        }
    }

    private IEnumerator WorkRoutine ()
    {
        while (_canRepair)
        {
            float elapsed = 0;

            while (elapsed < 1f / _attackSpeed.CurrentValue)
            {
                yield return null;

                if (_ship == null)
                {
                    _enemy.Rotator.SetTarget(_playerProvider.Player.Transform);
                    yield break;
                }

                elapsed += Time.deltaTime;
            }

            _enemy.EnemyAnimator.Attack(AttackCallback);
        }
    }

    private void AttackCallback ()
    {
        _soundSpawner.Spawn(_hitSound);
        _damageService.ApplyHeal(_ship, _enemy.StatHolder[StatType.Damage].CurrentValue, Color.green);
        _ship.EffectViewer.EnableEffect(EffectType.Heal);
    }
}
