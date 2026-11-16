using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunPharaohEyeLaserState : BossStateBase
{
    private const float AimHeight = 1f;

    private readonly SunPharaoh _boss;
    private IPlayerProvider _playerProvider;
    private ICoroutineRunner _coroutineRunner;
    private IDamageService _damageService;
    private IPoolService _poolService;
    private LineRenderer _lineL;
    private LineRenderer _lineR;
    private Coroutine _routine;
    private float _smoothedX;

    public SunPharaohEyeLaserState (SunPharaoh boss, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IPlayerProvider playerProvider, IDamageService damageService, IPoolService poolService)
    {
        _coroutineRunner = coroutineRunner;
        _playerProvider = playerProvider;
        _damageService = damageService;
        _poolService = poolService;
    }

    protected override void OnEnter ()
    {
        _boss.EnemyAnimator.PlayCast(() => _coroutineRunner.StartCoroutine(RunLaser()));
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<SunPharaohIdleState>();
    }

    protected override void OnExit ()
    {
        if (_routine != null)
        {
            _coroutineRunner.StopCoroutine(_routine);
            _routine = null;
        }

        HideLine(ref _lineL);
        HideLine(ref _lineR);
    }

    private IEnumerator RunLaser ()
    {
        var cfg = _boss.BossConfig;

        _lineL = EnsureLine(cfg.EyeLaserLinePrefab, "EyeLaser_L");
        _lineR = EnsureLine(cfg.EyeLaserLinePrefab, "EyeLaser_R");

        _smoothedX = 0;
        float elapsed = 0f;
        float sweepT = 1f;

        while (elapsed < Duration)
        {
            Vector3 playerPos = _playerProvider.Player.Transform.position;
            _smoothedX = Mathf.Lerp(_smoothedX, playerPos.x, cfg.EyeLaserFollowSpeed * Time.deltaTime);

            sweepT += Time.deltaTime * cfg.EyeLaserSweepSpeed;
            float sweep01 = Mathf.PingPong(sweepT / Mathf.Max(0.01f, Duration), 1f);
            float sweepAngle = Mathf.Lerp(-cfg.EyeLaserWidthDegrees * 0.5f,
                                           cfg.EyeLaserWidthDegrees * 0.5f,
                                           sweep01);

            Vector3 aimPoint = new Vector3(
                _smoothedX,
                AimHeight,
                playerPos.z);

            Vector3 dirL = GetAimDirection(_boss.EyeOriginLeft.position, aimPoint, sweepAngle);
            Vector3 dirR = GetAimDirection(_boss.EyeOriginRight.position, aimPoint, sweepAngle);

            CastAndDraw(_lineL, _boss.EyeOriginLeft.position, dirL, cfg);
            CastAndDraw(_lineR, _boss.EyeOriginRight.position, dirR, cfg);

            elapsed += Time.deltaTime;
            yield return null;
        }

        HideLine(ref _lineL);
        HideLine(ref _lineR);
        _routine = null;
        OnComplete();
    }

    private static Vector3 GetAimDirection (Vector3 origin, Vector3 aimPoint, float sweepAngle)
    {
        Vector3 toAim = aimPoint - origin;
        if (toAim.sqrMagnitude < 1e-4f)
            return Vector3.forward;

        Quaternion sweep = Quaternion.AngleAxis(sweepAngle, Vector3.up);
        return (sweep * toAim).normalized;
    }

    private void CastAndDraw (LineRenderer line, Vector3 origin, Vector3 dir, SunPharaohConfig cfg)
    {
        Vector3 endPoint = origin + dir * cfg.EyeLaserLength;

        if (Physics.Raycast(origin, dir, out var hit, cfg.EyeLaserLength, cfg.DamageMask))
        {
            endPoint = hit.point;

            if (hit.collider.TryGetComponent(out Player target))
                _damageService.ApplyDamage(target, cfg.EyeLaserDamagePerSecond * Time.deltaTime);
        }

        DrawLine(line, origin, endPoint);
    }

    private LineRenderer EnsureLine (LineRenderer prefab, string name)
    {
        if (prefab == null)
            return null;

        var line = _poolService.Get(prefab);
        line.name = name;
        line.positionCount = 2;
        line.useWorldSpace = true;
        return line;
    }

    private static void DrawLine (LineRenderer line, Vector3 start, Vector3 end)
    {
        if (line == null)
            return;

        line.enabled = true;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    private void HideLine (ref LineRenderer line)
    {
        if (line == null)
            return;

        _poolService.Release(line);
        line = null;
    }
}