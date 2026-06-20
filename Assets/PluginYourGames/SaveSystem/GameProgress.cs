using System;
using System.Collections.Generic;

namespace BigBalls.Saves
{
    [Serializable]

    public class GameProgress
    {
        public int Coins;
        public int Score;

        public List<ItemSaveData> Items = new();
        public List<BallSaveData> Balls = new();
        public List<SoundSaveData> Sounds = new();
    }
}