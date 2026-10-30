using System;

namespace BigBalls.Saves
{
    [Serializable]
    public class HelmetSaveData : ItemSaveData
    {
        public HelmetSaveData(int id, bool isOpen, float itemExp, int level = 1, bool hasPlayer = false) : base(id, isOpen, itemExp, level, hasPlayer)
        {
        }
    }
}