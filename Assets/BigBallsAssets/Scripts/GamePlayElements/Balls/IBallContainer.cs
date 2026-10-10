using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public interface IBallContainer
    {
        IEnumerable<BallModel> Balls { get; }
        Stat BallCount { get; }

        bool AddUniqueBall(BallModel uniqueBall);
        bool RemoveUniqueBall (BallModel ball);
        //void SetWeapon(WeaponModel weapon);
        void Subscribe();
        bool TryGetNextBullet(out Ball ball);
        void Unsubscribe();
    }
}