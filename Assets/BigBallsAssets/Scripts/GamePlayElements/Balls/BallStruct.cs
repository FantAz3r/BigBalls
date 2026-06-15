using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using UnityEngine;

public struct BallStruct
{
    public int ID;
    public BallConfig Config;
    public int Level;

    public BallStruct(int iD, BallConfig config, int level = 1)
    {
        ID = iD;
        Config = config;
        Level = level;
    }
}
