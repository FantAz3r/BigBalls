using BigBalls.Configs;
using UnityEngine;

[CreateAssetMenu(fileName = "SkeletonKingConfig", menuName = "Configs/SkeletonKingConfig")]
public class SkeletonKingConfig : EnemyConfig
{
    [Header("Idle")]
    public float IdleMinDuration = 1.5f;
    public float IdleMaxDuration = 2.5f;

    [Header("Bone Cloud")]
    public float BoneCloudDuration = 0.6f;
    public float BoneCloudSpreadAngle = 25f;
    public float BoneCloudShotDelay = 0.075f;
    public float VolleyDelay = 3;
    public int VolleyCount = 3;
    public int MinBoneCount = 20;
    public int MaxBoneCount = 30;

    public GameObject BoneCloudEffect;

    [Header("Hand Attack")]
    public float HandDamage = 15f;
    public float HandAttackDuration = 2.5f;
    public float HandFlyDistance = 2.5f;
    public float HandTravelTime = 0.6f;       
    public float HandReturnTime = 0.6f;

    [Header("Bone Fan")]
    public float FanBoneDuration = 2.5f;
    public int FanBoneCount = 7;
    public float FanShotInterval = 0.15f;
    public float FanSpreadAngle = 90f;
    public float FanSpawnOffset = 0.8f;
}