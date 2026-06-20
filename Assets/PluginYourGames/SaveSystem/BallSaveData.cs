using System;

[Serializable]
public class BallSaveData
{
    public int BallType;
    public float Damage;
    public bool IsOpen;
    public int Level;

    public BallSaveData(int ballType, float damage, bool isOpen, int level = 1)
    {
        BallType = ballType;
        Damage = damage;
        IsOpen = isOpen;
        Level = level;
    }
}
