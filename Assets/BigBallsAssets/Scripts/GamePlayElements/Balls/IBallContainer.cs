namespace BigBalls.GameplayObjects
{
    public interface IBallContainer
    {
        void Subscribe();
        bool TryGetNextBullet(out Ball ball);
        void Unsubscribe();
    }
}