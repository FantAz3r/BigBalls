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

        public string Language = "en";
        public bool ShowDamageNumbers = true;
        public bool ShowFPS = false;
        public bool AutoAttack = true;
        public void ClearProgress()
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

        public void ResetSettings()
        {
            Language = "en";
            ShowDamageNumbers = true;
            ShowFPS = false;
            AutoAttack = true;
        }
    }
}