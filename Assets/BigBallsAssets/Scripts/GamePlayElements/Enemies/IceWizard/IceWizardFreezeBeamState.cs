using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class IceWizardFreezeBeamState : BossStateBase
{
    private const float DamageTickInterval = 0.25f;
    private const float MinDirectionSqrMagnitude = 0.0001f;
    private const float MinAimDistance = 0.01f;
    private const float SlowDownMinFactor = 0.2f;

    private readonly IceWizardBoss _boss;
    private IPlayerProvider _playerProvider;
    private ICoroutineRunner _coroutineRunner;
    private IDamageService _damageService;
    private DebuffService _debuffService;
    private Coroutine _routine;

    private RaycastHit _lastHit;
    private bool _lastHitValid;

    private float _currentYaw;
    private float _startYaw;
    private float _freezeProgress;

    private float _damageAccumulator;
    private float _damageTimer;

    public IceWizardFreezeBeamState (IceWizardBoss boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (
        IPlayerProvider playerProvider,
        ICoroutineRunner runner,
        DebuffService debuffService,
        IDamageService damageService)
    {
        _playerProvider = playerProvider;
        _coroutineRunner = runner;
        _debuffService = debuffService;
        _damageService = damageService;
    }

    protected override void OnEnter ()
    {
        _freezeProgress = 0f;
        _lastHitValid = false;

        Vector3 aimDirection = GetInitialAimDirection();

        _startYaw = _currentYaw = DirectionToYaw(aimDirection);

        _boss.EnemyAnimator.Attack(OnAnimationEvent);
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<IceWizardIdleState>();
    }

    protected override void OnExit ()
    {
        StopRoutine();

        if (_boss.IceBeam != null)
            _boss.IceBeam.Stop();

        _lastHitValid = false;
    }

    private void OnAnimationEvent ()
    {
        _routine = _coroutineRunner.StartCoroutine(Run());
    }

    private IEnumerator Run ()
    {
        var config = _boss.BossConfig;
        Vector3 beamOrigin = _boss.BeamOrigin.position;
        _boss.IceBeam.Play();
        float elapsed = 0f;

        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;

            UpdateAim(config, beamOrigin, Time.deltaTime);
            UpdateBeam(config, beamOrigin);
            UpdateFreeze(config, beamOrigin, Time.deltaTime);

            yield return null;
        }
    }

    private void UpdateAim (IceWizardBossConfig config, Vector3 beamOrigin, float deltaTime)
    {
        if (TryGetPlayerPosition(out Vector3 playerPosition) == false)
            return;

        Vector3 toPlayer = playerPosition - beamOrigin;
        toPlayer.y = 0f;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer < MinAimDistance)
            return;

        float targetYaw = DirectionToYaw(toPlayer);
        float yawDeltaFromStart = Mathf.DeltaAngle(_startYaw, targetYaw);

        if (Mathf.Abs(yawDeltaFromStart) > config.FreezeBeamMaxYawOffset)
            targetYaw = _startYaw + Mathf.Sign(yawDeltaFromStart) * config.FreezeBeamMaxYawOffset;

        float angleToTarget = Mathf.DeltaAngle(_currentYaw, targetYaw);

        if (Mathf.Abs(angleToTarget) < config.FreezeBeamDeadZoneDeg)
        {
            _currentYaw = targetYaw;
            return;
        }

        float safeDistance = Mathf.Max(distanceToPlayer, config.FreezeBeamMinDistance);
        float turnRateDegPerSec = config.FreezeBeamLinearSpeed / safeDistance * Mathf.Rad2Deg;

        float slowDownFactor = Mathf.Min(1f, Mathf.Abs(angleToTarget) / config.FreezeBeamSlowDownAngleDeg);
        slowDownFactor = Mathf.Max(SlowDownMinFactor, slowDownFactor);

        float stepDegrees = turnRateDegPerSec * slowDownFactor * deltaTime;
        _currentYaw = Mathf.MoveTowardsAngle(_currentYaw, targetYaw, stepDegrees);
    }

    private void UpdateBeam (IceWizardBossConfig config, Vector3 beamOrigin)
    {
        if (_boss.IceBeam == null)
            return;

        Vector3 beamDirection = YawToDirection(_currentYaw);
        float beamLength = config.FreezeBeamLength;

        _lastHitValid = Physics.Raycast(beamOrigin, beamDirection, out _lastHit, beamLength, config.FreezeBeamMask);

        if (_lastHitValid)
            beamLength = _lastHit.distance;

        Transform beamTransform = _boss.IceBeam.transform;
        beamTransform.position = beamOrigin;
        beamTransform.rotation = Quaternion.LookRotation(beamDirection, Vector3.up);
    }

    private void UpdateFreeze (IceWizardBossConfig config, Vector3 beamOrigin, float deltaTime)
    {
        if (!TryGetPlayer(out Player player))
            return;

        bool isPlayerInBeam = IsPlayerHitByBeam();

        if (isPlayerInBeam)
        {
            UpdateFreezeProgress(config, player, deltaTime);
            UpdateBeamDamage(config, player, deltaTime);
        }
        else
        {
            DecayFreezeProgress(config, deltaTime);
        }
    }

    private void UpdateFreezeProgress (IceWizardBossConfig config, Player player, float deltaTime)
    {
        _freezeProgress += deltaTime * config.FreezeProgressPerSec;

        if (_freezeProgress < config.FreezeStacksToStun)
            return;

        _debuffService.Cancel(player, EffectType.Freeze);
        _debuffService.Apply(EffectType.Stun, player, config.FreezeStunDuration);
        _freezeProgress = 0f;
    }

    private void UpdateBeamDamage (IceWizardBossConfig config, Player player, float deltaTime)
    {
        _damageAccumulator += deltaTime * config.FreezeBeamDPS;
        _damageTimer += deltaTime;

        if (_damageTimer < DamageTickInterval)
            return;

        _damageService.ApplyDamage(player, _damageAccumulator, Color.blue);

        _damageAccumulator = 0f;
        _damageTimer = 0f;
    }

    private void DecayFreezeProgress (IceWizardBossConfig config, float deltaTime)
    {
        _freezeProgress = Mathf.Max(0f, _freezeProgress - config.FreezeProgressDecayPerSec * deltaTime);

        _damageAccumulator = 0f;
        _damageTimer = 0f;
    }

    private bool IsPlayerHitByBeam ()
    {
        if (_lastHitValid == false)
            return false;

        if (_lastHit.collider.TryGetComponent(out IEntity entity) == false)
            return false;

        return entity is Player;
    }

    private Vector3 GetInitialAimDirection ()
    {
        Vector3 origin = _boss.BeamOrigin.position;

        Vector3 forward = _boss.BeamOrigin.forward;
        forward.y = 0f;

        if (TryGetPlayerPosition(out Vector3 playerPosition))
        {
            Vector3 toPlayer = playerPosition - origin;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > MinDirectionSqrMagnitude)
                forward = toPlayer.normalized;
        }

        if (forward.sqrMagnitude < MinDirectionSqrMagnitude)
            forward = Vector3.forward;

        return forward;
    }

    private void StopRoutine ()
    {
        if (_routine == null)
            return;

        _coroutineRunner.StopCoroutine(_routine);
        _routine = null;
    }

    private bool TryGetPlayerPosition (out Vector3 position)
    {
        if (TryGetPlayer(out var player))
        {
            position = player.Transform.position;
            return true;
        }

        position = default;
        return false;
    }

    private bool TryGetPlayer (out Player player)
    {
        player = _playerProvider?.Player;
        return player != null;
    }

    private static float DirectionToYaw (Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    private static Vector3 YawToDirection (float yawDegrees)
    {
        float yawRadians = yawDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(yawRadians), -0.1f, Mathf.Cos(yawRadians));
    }
}