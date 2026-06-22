namespace BigBalls.GameplayObjects
{
    public interface IBallContainer
    {
        bool AddUniqueBall (BallModel uniqueBall);
        void Subscribe();
        bool TryGetNextBullet(out Ball ball);
        void Unsubscribe();
    }
}