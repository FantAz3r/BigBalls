using System;

namespace BigBalls.Saves
{
    [Serializable]
    public class CardSaveData
    {
        public int Id;
        public int NoneGameLevel;
        public bool IsOpen;
        public float ItemExp;

        public CardSaveData (int id, bool isOpen, float itemExp, int level = 1)
        {
            Id = id;
            NoneGameLevel = level;
            IsOpen = isOpen;
            ItemExp = itemExp;
        }
    }
}