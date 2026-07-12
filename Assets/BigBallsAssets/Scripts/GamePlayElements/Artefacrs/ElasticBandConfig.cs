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
    public float MaxMoveSpeed;

    public override BehaviourType Type => BehaviourType.ElasticBand;

    public override List<ItemStat> GetStats(int level)
    {
        throw new NotImplementedException();
    }
}