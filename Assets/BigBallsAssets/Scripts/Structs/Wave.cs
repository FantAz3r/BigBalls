using BigBalls.Configs;
using System;
using System.Collections.Generic;

[Serializable]
public struct Wave
{
    public int WaveCooldown;
    public EnemyConfig BossConfig;
    public int LineCount;
    public List<EnemyConfig> Enemies;
}
