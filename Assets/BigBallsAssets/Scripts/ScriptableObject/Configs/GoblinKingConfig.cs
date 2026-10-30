using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using UnityEngine;

[CreateAssetMenu(fileName = "GoblinKingConfig", menuName = "Configs/GoblinKingConfig")]
public class GoblinKingConfig : EnemyConfig
{
    [Header("Idle / Defensive Stance")]
    public float IdleMinDuration = 1.2f;
    public float IdleMaxDuration = 2.2f;
    public float BlockStanceDuration = 1.5f; // сколько держит блок перед атакой

    [Header("Axe Fan (веер топоров)")]
    public float AxeFanDuration = 1.5f;
    public int AxeFanCount = 7;                // 5-7
    public float AxeFanSpreadAngle = 80f;      // общий угол веера
    public float AxeFanDamage = 12f;
    public float AxeFanProjectileSpeed = 7f;

    [Header("Boomerang Axes (два бумеранга из рук)")]
    public float BoomerangDuration = 2.8f;
    public float BoomerangFlightTime = 1.2f;   // время полёта до точки возврата
    public float BoomerangReturnTime = 1.0f;   // время возврата в руку
    public float BoomerangDamage = 20f;
    public float BoomerangRadius = 1.2f;       // радиус эллиптической орбиты
    public float BoomerangEllipseB = 0.6f;     // малая полуось эллипса
    public float BoomerangArcHeight = 0.4f;    // высота дуги

    [Header("Roar (оглушающий рёв)")]
    public float RoarDuration = 1.4f;
    public float RoarStunDuration = 2.5f;
    public float RoarRadius = 8f;
    public GameObject RoarEffect;
}
