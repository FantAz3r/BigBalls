namespace BigBalls.GameplayObjects
{
    public interface IUpgradable
    {
        int Level { get; }
        void Upgrade();
        void ResetLevel();
    }
}