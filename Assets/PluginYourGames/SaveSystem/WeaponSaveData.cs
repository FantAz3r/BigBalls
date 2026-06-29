using System;

namespace BigBalls.Saves
{
    [Serializable]
    public class WeaponSaveData : ItemSaveData
    {
        public WeaponSaveData(int id, bool isOpen, float itemExp, int level = 1) : base(id, isOpen, itemExp, level)
        {
        }
    }
}