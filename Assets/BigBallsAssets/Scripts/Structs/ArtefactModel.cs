using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using UnityEngine;

public class ArtefactModel : ICard
{
    public int ID;
    public ArtefactConfig Config;

    public ArtefactModel(int iD, ArtefactConfig config, int level = 1)
    {
        ID = iD;
        Config = config;
        Level = level;
    }

    public Sprite Icon => Config.Icon;

    public string Name => Config.name;

    public string Description => string.Empty;
    public int Level { get; private set; }

    public void Upgrade()
    {
        Level++;
    }
}
