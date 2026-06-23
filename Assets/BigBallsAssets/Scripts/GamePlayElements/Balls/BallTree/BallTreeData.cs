using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.StaticData;
using UnityEngine;

[CreateAssetMenu(fileName = "BallTreeData", menuName = "Game/Ball Tree Data")]
public class BallTreeData : ScriptableObject
{
    public List<BallConfig> Nodes;
    public BallType RootBallType;
    public Vector2 Spacing = new Vector2(150f, 100f);
    public float VerticalSpacing = 80f;
}