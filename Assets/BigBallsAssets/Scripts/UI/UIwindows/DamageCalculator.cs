using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

namespace BigBalls.UI
{
    public class DamageCalculator
    {
        private Dictionary<Ball, BallType> _ballInfos = new();

        public void Add (Ball ball)
        {
            BallType ballType = ball.Config.BallType;
            _ballInfos.Add(ball, ball.Config.BallType);
        }

        public void EndCalculate ()
        {
            _ballInfos.Clear();
        }

        public float GetDamage ()
        {
            float damage = 0f;

            foreach (var pair in _ballInfos)
            {
                damage += pair.Key.TotalDamage;
            }

            return damage;
        }
    }
}