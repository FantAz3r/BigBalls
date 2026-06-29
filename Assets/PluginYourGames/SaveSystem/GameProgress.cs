using System;
using System.Collections.Generic;

namespace BigBalls.Saves
{
    [Serializable]

    public class GameProgress
    {
        public int Coins;
        public int Score;

        public List<WeaponSaveData> Weapons = new();
        public List<ArmorSaveData> Armors = new();
        public List<HelmetSaveData> Helmets = new();

        public List<BallSaveData> Balls = new();
        public List<ArtefactSaveData> Artefacts = new();
        public List<SoundSaveData> Sounds = new();

        public void Clear()
        {
            Coins = 0;
            Score = 0;
            Weapons.Clear();
            Armors.Clear();
            Helmets.Clear();
            Balls.Clear();
            Artefacts.Clear();
            Sounds.Clear();
        }
    }
}