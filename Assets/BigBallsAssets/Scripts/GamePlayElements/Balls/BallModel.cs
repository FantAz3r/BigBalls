using System;
using BigBalls.Configs;
using UnityEngine;

public class BallModel : ICard
{
    public int ID;
    public BallConfig Config;
    public int Level;
    public event Action Upgraded;
    public BallModel(int iD, BallConfig config, int level = 1)
    {
        ID = iD;
        Config = config;
        Level = level;
    }

    public Sprite Icon => Config.Icon;

    public string Name => Config.name;

    public string Description => string.Empty;

    int ICard.Level => Level;

    public void Upgrade() => Level++;
}
