using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunPharaohSolarStreamsState : BossStateBase
{
    private const int BeamHeight = 20;

    private readonly SunPharaoh _boss;
    private IPoolService _poolService;
    private IPlayerProvider _playerProvider;
    private ICoroutineRunner _coroutineRunner;
    private Coroutine _spawnRoutine;
    private WaitForSeconds _wait;
    private bool _canCast;

    public SunPharaohSolarStreamsState (SunPharaoh boss, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (IPoolService poolService, IPlayerProvider playerProvider, ICoroutineRunner coroutineRunner)
    {
        _poolService = poolService;
        _playerProvider = playerProvider;
        _coroutineRunner = coroutineRunner;
    }

    protected override void OnEnter ()
    {
        _boss.EnemyAnimator.Attack(OnCast);
    }

    private void OnCast ()
    {
        _canCast = true;
        _wait = new WaitForSeconds (_boss.BossConfig.SpawnInterval);
        _spawnRoutine = _coroutineRunner.StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop ()
    {
        while (_canCast)
        {
            SpawnBeamAtPlayer();
            yield return _wait;
        }
    }

    private void SpawnBeamAtPlayer ()
    {
        if (_playerProvider.Player?.Transform == null)
            return;

        Vector3 playerPos = _playerProvider.Player.Transform.position;
        Vector3 world = new Vector3(playerPos.x, BeamHeight, playerPos.z);

        SolarBeam beam = _poolService.Get(_boss.BossConfig.SolarBeamPrefab, world);
        beam.Init(
            windup: _boss.BossConfig.SolarStreamsWindup,
            active: _boss.BossConfig.BeamCastDuration,
            damage: _boss.BossConfig.SunOrbBeamDamage,
            mask: _boss.BossConfig.SunOrbBeamMask);
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<SunPharaohIdleState>();
    }

    protected override void OnExit ()
    {
        _canCast = false;

        if (_spawnRoutine != null)
        {
            _coroutineRunner.StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }
}