using BigBalls.Configs;
using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/IceWizardConfig", fileName = "IceWizardConfig")]
public class IceWizardBossConfig : EnemyConfig
{
    [Header("Idle")]
    public float IdleMinDuration = 1.2f;
    public float IdleMaxDuration = 2.0f;

    [Header("1. Freeze Beam")]
    public float FreezeBeamDuration = 4f;
    public float FreezeBeamLength = 20f;
    public LayerMask FreezeBeamMask;
    public int FreezeStacksToStun = 5;
    public float FreezeStunDuration = 1.5f;

    public float FreezeBeamMaxYawOffset = 90f;
    public float FreezeBeamLinearSpeed = 5f;
    public float FreezeBeamMinDistance = 1.5f;
    public float FreezeBeamDeadZoneDeg = 1.5f;
    public float FreezeBeamSlowDownAngleDeg = 15f;
    public float FreezeBeamDPS = 10f;

    public float FreezeProgressPerSec = 1f;
    public float FreezeProgressDecayPerSec = 0.6f;

    [Header("2. Cold Nova")]
    public float ColdNovaDuration = 2.0f;
    public float ColdNovaWindup = 0.4f;
    public float ColdNovaRadius = 6f;
    public float ColdNovaShieldAmount = 10f;
    public float ColdNovaShieldDuration = 6f;
    public float ColdNovaStunDuration = 2.5f;
    public LayerMask ColdNovaMask;

    [Header("3. Ice Ball")]
    public float IceBallDuration = 2.0f;
    public float IceBallShieldAmount = 50f;
    public LayerMask IceBallMask;

    [Header("4. Icicle Rain")]
    public float IcicleRainDuration = 5f;
    public int IcicleMin = 15;
    public int IcicleMax = 20;
    public Icicle IciclePrefab;
    public float IcicleDamage = 20f;
    public float IcicleFallTime = 0.6f;
    public float IcicleTelegraphTime = 0.5f;
    public LayerMask IcicleDamageMask;
}
