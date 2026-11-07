using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunPharaohEyeLaserState : BossStateBase
{
    private readonly SunPharaoh _boss;
    private IPlayerProvider _playerProvider;
    private IUpdateService _updateService;
    private ICoroutineRunner _coroutineRunner;
    private LineRenderer _lineL;
    private LineRenderer _lineR;
    private Coroutine _routine;

    public SunPharaohEyeLaserState (SunPharaoh boss, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (IUpdateService updateService, ICoroutineRunner coroutineRunner, IPlayerProvider playerProvider)
    {
        _updateService = updateService;
        _coroutineRunner = coroutineRunner;
        _playerProvider = playerProvider;
    }

    protected override void OnEnter ()
    {
        _routine = _coroutineRunner.StartCoroutine(RunLaser());
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

        yield return new WaitForSeconds(cfg.EyeLaserWindup);

        float elapsed = 0f;
        while (elapsed < Duration)
        {
            Vector3 targetPoint = _playerProvider.Player.Transform.position;
            targetPoint.y = 0f;

            // Ограничение угла: каждое «око» стреляет в своей половине сектора
            Vector3 dirL = ClampToSector(cfg.EyeOriginLeftOrigin(), cfg.EyeLaserWidthDegrees, 0f, targetPoint);
            Vector3 dirR = ClampToSector(cfg.EyeOriginRightOrigin(), cfg.EyeLaserWidthDegrees, 0f, targetPoint);

            DrawLaser(_lineL, cfg.EyeOriginLeftOrigin(), dirL, cfg.EyeLaserLength);
            DrawLaser(_lineR, cfg.EyeOriginRightOrigin(), dirR, cfg.EyeLaserLength);

            ApplyLaserDamage(cfg.EyeOriginLeftOrigin(), dirL, cfg.EyeLaserLength, cfg, Time.deltaTime);
            ApplyLaserDamage(cfg.EyeOriginRightOrigin(), dirR, cfg.EyeLaserLength, cfg, Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        HideLine(ref _lineL);
        HideLine(ref _lineR);
        _routine = null;
        OnComplete();
    }

    private static Vector3 ClampToSector (Transform origin, float widthDeg, float biasDeg, Vector3 targetWorld)
    {
        Vector3 fwd = origin.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 toTarget = targetWorld - origin.position; toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 1e-4f) return fwd;

        float targetAngle = Vector3.SignedAngle(fwd, toTarget, Vector3.up);
        float clamped = Mathf.Clamp(targetAngle + biasDeg, -widthDeg * 0.5f, widthDeg * 0.5f);
        return Quaternion.AngleAxis(clamped, Vector3.up) * fwd;
    }

    private static LineRenderer EnsureLine (LineRenderer prefab, string name)
    {
        if (prefab == null)
            return null;

        var line = Object.Instantiate(prefab);
        line.name = name;
        line.positionCount = 2;
        line.useWorldSpace = true;
        return line;
    }

    private static void DrawLaser (LineRenderer line, Transform origin, Vector3 dir, float length)
    {
        if (line == null)
            return;

        line.enabled = true;
        line.SetPosition(0, origin.position);
        line.SetPosition(1, origin.position + dir * length);
    }

    private static void HideLine (ref LineRenderer line)
    {
        if (line == null)
            return;

        Object.Destroy(line.gameObject);
        line = null;
    }

    private static void ApplyLaserDamage (Vector3 origin, Vector3 dir, float length, SunPharaohConfig cfg, float dt)
    {
        if (Physics.Raycast(origin, dir, out var hit, length, cfg.DamageMask))
        {
            if (hit.collider.TryGetComponent(out IDamageable target))
                target.TakeDamage(cfg.EyeLaserDamagePerSecond * dt);
        }
    }
}