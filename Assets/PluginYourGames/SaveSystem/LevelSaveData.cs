using UnityEditor.Experimental.GraphView;

namespace BigBalls.Saves
{
    public class LevelSaveData
    {
        private const int MaxWaves = 3;
        public int LevelID = 4;
        public int CompliteWaves;
        public int Kills;
        public float BestDamage;
        public float Time;
        public bool IsComplite;

        public LevelSaveData(int levelID, int compliteWaves = 0, float time = 0, int kills = 0, float bestDamage = 0)
        {
            LevelID = levelID;
            CompliteWaves = compliteWaves;
            BestDamage = bestDamage;
            Kills = kills;
            Time = time;
            IsComplite = CompliteWaves >= MaxWaves;
        }
    }
}