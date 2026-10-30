using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "ElasticBandConfig", menuName = "Configs/BahaviourConfig/ElasticBandConfig")]
[Serializable]

public class ElasticBandConfig : EffectConfig
{
    public float StartBallMoveSpeed;
    public float AddMoveSpeedPerHit;
    public float MaxMoveSpeedMultiplyer;

    public float AddMoveSpeedPerHitPerLevel;
    public float MaxMoveSpeedMultiplyerPerLevel;
    public override BehaviourType Type => BehaviourType.ElasticBand;

    public float GetStartMoveSpeed (int level)
    {
        float result = AddMoveSpeedPerHit + AddMoveSpeedPerHitPerLevel * level;
        return result;
    }

    public float GetSpeedMyltiplyer (int level) => MaxMoveSpeedMultiplyer + MaxMoveSpeedMultiplyerPerLevel * level;

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.MaxMoveSpeedMultiplyer, GetStartMoveSpeed(level), GetStartMoveSpeed(nextLevel)),
            new ItemStat(TextLocalizator.AddMoveSpeedPerHit, GetSpeedMyltiplyer(level), GetSpeedMyltiplyer(nextLevel)),
            new ItemStat(TextLocalizator.StartBallMoveSpeed, StartBallMoveSpeed)
        };
    }
}