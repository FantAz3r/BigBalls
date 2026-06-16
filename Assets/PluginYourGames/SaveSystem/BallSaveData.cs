public struct BallSaveData
{
    public int BallType;
    public int Damage;
    public bool IsOpen;
    public int Level;

    public BallSaveData(int ballType, int damage, bool isOpen, int level = 1)
    {
        BallType = ballType;
        Damage = damage;
        IsOpen = isOpen;
        Level = level;
    }
}
