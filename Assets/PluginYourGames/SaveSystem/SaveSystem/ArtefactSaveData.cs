using System;

namespace BigBalls.Saves
{
    [Serializable]
    public class ArtefactSaveData : CardSaveData
    {
        public ArtefactSaveData (int id, bool isOpen, float itemExp, int level = 1, bool hasPlayer = false) : base(id, isOpen, itemExp, level, hasPlayer)
        {
        }
    }
}