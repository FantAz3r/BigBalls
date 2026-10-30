using System;
using BigBalls.Configs;

namespace BigBalls.Saves
{
    [Serializable]
    public class WeaponSaveData : ItemSaveData
    {
        public WeaponSaveData(int id, bool isOpen, float itemExp, int level = 1, bool hasPlayer = false) : base(id, isOpen, itemExp, level, hasPlayer)
        {
        }
    }
}