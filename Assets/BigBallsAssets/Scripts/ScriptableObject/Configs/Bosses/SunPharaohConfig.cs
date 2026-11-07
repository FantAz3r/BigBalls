using UnityEngine;

[CreateAssetMenu(menuName = "Bosses/SunPharaohConfig", fileName = "SunPharaohConfig")]
public class SunPharaohConfig : ScriptableObject
{
    [Header("Idle / timing")]
    public float IdleMinDuration = 1.2f;

    [Header("1. Eye Laser")]
    public float EyeLaserDuration = 3.5f;      // общее время каста
    public float EyeLaserWindup = 0.6f;        // подготовка перед первым лучом
    public float EyeLaserWidthDegrees = 18f;   // 15-20 градусов
    public float EyeLaserLength = 20f;
    public float EyeLaserDamagePerSecond = 25f;
    public LayerMask DamageMask;               // слой игрока + препятствий
    public Transform EyeLaserOriginRight;      // точки глаз
    public Transform EyeLaserOriginLeft;
    public LineRenderer EyeLaserLinePrefab;    // опционально; иначе — VFX

    [Header("2. Sun Orb")]
    public float SunOrbDuration = 8f;          // общее время жизни способности
    public GameObject SunOrbPrefab;            // префаб с SunOrb
    public float SunOrbRotationSpeed = 90f;    // град/сек
    public float SunOrbBeamInterval = 0.35f;   // как часто стреляет лучами
    public int SunOrbBeamsPerVolley = 6;       // лучей за залп
    public float SunOrbBeamLength = 12f;
    public float SunOrbBeamDamage = 12f;
    public float SunOrbHealPerSecond = 3f;     // хил боссу
    public float SunOrbAccuracyDebuff = 0.15f; // -15% точности игроку
    public float SunOrbAccuracyDebuffDuration = 4f;
    public LayerMask SunOrbBeamMask;

    [Header("3. Solar Streams")]
    public float SolarStreamsDuration = 5f;
    public float SolarStreamsWindup = 0.5f;    // 0.5с показываем зону
    public int GridWidth = 15;                 // 15x15
    public int GridHeight = 15;
    public int CellsPerBeam = 4;               // 1 луч на 4 клетки
    public int BeamCount = 3;                  // 3 луча -> покрывают ~ 1/3 (15*15/4)
    public float BeamCastDuration = 1.2f;      // длительность самого удара
    public GameObject SolarBeamPrefab;         // префаб с SolarBeam
    public GameObject SolarBeamTelegraphPrefab;// индикатор зоны (0.5с)
    public float CellSize = 1f;
}