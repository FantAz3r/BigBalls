using BigBalls.Configs;
using System;

[Serializable]
public struct Wave 
{
    public int WaveCooldown;
    public EnemyConfig BossConfig;
    public int LineCount;
}
