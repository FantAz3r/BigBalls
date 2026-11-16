namespace BigBalls.Saves
{
    public class LevelSaveData
    {
        private const int MaxWaves = 3;
        public int LevelID = 4;
        public int Score;
        public int Stars;
        public int Kills;
        public float BestDamage;
        public float Time;
        public bool IsComplite;

        public LevelSaveData(int levelID, int stars, int score = 0, float time = 0, int kills = 0, float bestDamage = 0)
        {
            LevelID = levelID;
            Score = score;
            BestDamage = bestDamage;
            Kills = kills;
            Time = time;
            Stars = stars;
            IsComplite = Score >= MaxWaves;
        }
    }
}