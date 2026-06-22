using System;

namespace BigBalls.Saves
{
    [Serializable]

    public class ItemSaveData
    {
        public int ItemType;
        public int CardCount;
        public bool HasPlayer;
    }
}