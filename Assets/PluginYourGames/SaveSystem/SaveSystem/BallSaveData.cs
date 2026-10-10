using System;

namespace BigBalls.Saves
{
    [Serializable]
    public class BallSaveData : CardSaveData
    {
        public BallSaveData (int ballType, float damage, bool isOpen, int level = 1) : base(ballType, isOpen, damage, level)
        {
        }
    }
}
