using BigBalls.Configs;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Bosses/SunPharaohConfig", fileName = "SunPharaohConfig")]
public class SunPharaohConfig : EnemyConfig
{
    [Header("Idle / timing")]
    public float IdleMinDuration = 1.2f;

    [Header("1. Eye Laser")]
    public float EyeLaserDuration = 3.5f;      // общее время каста
    public float EyeLaserWindup = 0.6f;        // подготовка перед первым лучом
    public float EyeLaserWidthDegrees = 18f;   // 15-20 градусов
    public float EyeLaserLength = 20f;
    public float EyeLaserDamagePerSecond = 25f;
    public float EyeLaserSweepSpeed = 0.5f;
    public LayerMask DamageMask;               // слой игрока + препятствий
    public LineRenderer EyeLaserLinePrefab;    // опционально; иначе — VFX
    public float EyeLaserFollowSpeed = 3f;

    [Header("2. Sun Orb")]

    public SunOrbConfig SunOrbConfig;
    public float SunOrbDuration = 8f;          // общее время жизни способности
    public float SunOrbPreviewOffset = 1f;          // общее время жизни способности
    public float SunOrbRotateDuration = 2f;          // общее время жизни способности
    public float SunOrbPrepareDuration = 0.5f;          // общее время жизни способности
    public float SunOrbBeamInterval = 0.35f;   // как часто стреляет лучами
    public int SunOrbBeamsPerVolley = 6;       // лучей за залп
    public float SunOrbBeamLength = 12f;
    public float SunOrbRotationSpeed = 90f;
    public float SunOrbBeamDamage = 10f;
    public float SunOrbAccuracyDebuff = 15f; // -15% точности игроку
    public float SunOrbAccuracyDebuffDuration = 4f;
    public LayerMask SunOrbBeamMask;

    [Header("3. Solar Streams")]
    public float SolarStreamsDuration = 5f;
    public float SolarStreamsWindup = 0.5f;    // 0.5с показываем зону
    public int BeamCount = 3;                  // 3 луча -> покрывают ~ 1/3 (15*15/4)
    public float BeamCastDuration = 1.2f;      // длительность самого удара
    public SolarBeam SolarBeamPrefab;         // префаб с SolarBeam
    public GameObject SolarBeamTelegraphPrefab;// индикатор зоны (0.5с)
    public float SpawnInterval = 0.5f;
}