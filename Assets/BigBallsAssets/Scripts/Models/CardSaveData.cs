using System;

namespace BigBalls.StaticData
{
    [Serializable]
    public class CardSaveData
    {
        public int Id;
        public int Level;
        public bool IsOpen;
        public float ItemExp;

        public CardSaveData (int id, bool isOpen, float itemExp, int level = 1)
        {
            Id = id;
            Level = level;
            IsOpen = isOpen;
            ItemExp = itemExp;
        }
    }
}