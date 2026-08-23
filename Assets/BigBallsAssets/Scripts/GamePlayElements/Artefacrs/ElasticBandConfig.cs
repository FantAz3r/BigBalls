using BigBalls.Configs;
using BigBalls.GameplayObjects;
using System;
using System.Collections.Generic;
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


    public float GetMoveSpeed(int level) => AddMoveSpeedPerHit + AddMoveSpeedPerHitPerLevel * level;
    public float GetMaxSpeedMyltiplyer(int level) => MaxMoveSpeedMultiplyer + MaxMoveSpeedMultiplyerPerLevel * level;

    public override List<ItemStat> GetStats(int level)
    {
        throw new NotImplementedException();
    }
}