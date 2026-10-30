using System;

namespace BigBalls.Saves
{
    [Serializable]
    public class CardSaveData
    {
        public int Id;
        public int NoneGameLevel;
        public bool IsOpen;
        public bool HasPlayer;
        public float ItemExp;

        public CardSaveData (int id, bool isOpen,  float itemExp, int level = 1, bool hasPlayer = false)
        {
            Id = id;
            NoneGameLevel = level;
            IsOpen = isOpen;
            ItemExp = itemExp;
            HasPlayer = hasPlayer;
        }
    }
}