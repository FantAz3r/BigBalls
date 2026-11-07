using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "PhantomConfig", menuName = "Configs/BahaviourConfig/PhantomConfig")]
[Serializable]
public class PhantomConfig : EffectConfig
{
    [Header("Speed on kill")]
    public float SpeedBonusPerKill = 0.15f;
    public float SpeedBonusPerKillPerLevel = 0.05f;
    public float MaxSpeedBonus = 1.0f;

    [Header("Curse")]
    public int HitsToProc = 5;
    public int HitsToProcPerLevel = -1;
    public int MinHitsToProc = 2;

    public float CurseDamageMultiplier = 10f;

    [Header("Visual")]
    public Color CurseColor = new Color(0.7f, 0.2f, 1f);

    public override BehaviourType Type => BehaviourType.Phantom;

    public float GetSpeedBonusPerKill (int level)
        => Utils.Math.Additive(SpeedBonusPerKill, SpeedBonusPerKillPerLevel, level);

    public float GetMaxSpeedBonus (int level) => MaxSpeedBonus;

    public int GetHitsToProc (int level)
    {
        int raw = HitsToProc + HitsToProcPerLevel * level;
        return System.Math.Max(MinHitsToProc, raw);
    }

    public override List<ItemStat> GetStats (int level)
    {
        int next = level + 1;

        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.SpeedPerKill, GetSpeedBonusPerKill(level) * 100f, GetSpeedBonusPerKill(next) * 100f, true),
            new ItemStat(TextLocalizator.HitsToProc,  GetHitsToProc(level), GetHitsToProc(next)),
            new ItemStat(TextLocalizator.DamageMultiply,  CurseDamageMultiplier)
        };
    }
}
