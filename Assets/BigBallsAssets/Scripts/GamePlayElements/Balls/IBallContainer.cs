using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public interface IBallContainer
    {
        IEnumerable<BallModel> Balls { get; }

        bool AddUniqueBall(BallModel uniqueBall);
        void AddUniqueBall(WeaponModel weapon);
        void Subscribe();
        bool TryGetNextBullet(out Ball ball);
        void Unsubscribe();
    }
}