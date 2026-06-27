using System;

namespace BigBalls.Saves
{
    [Serializable]

    public class ItemSaveData : CardSaveData
    {
        public ItemSaveData(int id, bool isOpen, float itemExp, int level = 1) : base(id, isOpen, itemExp, level)
        {
        }
    }
}