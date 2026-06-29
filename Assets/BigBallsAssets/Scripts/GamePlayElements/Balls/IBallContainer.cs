namespace BigBalls.GameplayObjects
{
    public interface IBallContainer
    {
        bool AddUniqueBall (BallModel uniqueBall);
        void AddUniqueBall(WeaponModel weapon);
        void Subscribe();
        bool TryGetNextBullet(out Ball ball);
        void Unsubscribe();
    }
}