using System;

namespace BigBalls.Saves
{
    [Serializable]
    public class ArmorSaveData : ItemSaveData
    {
        public ArmorSaveData(int id, bool isOpen, float itemExp, int level = 1) : base(id, isOpen, itemExp, level)
        {
        }
    }
}