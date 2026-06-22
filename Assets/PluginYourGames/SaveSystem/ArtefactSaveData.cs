using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Saves
{
    public class ArtefactSaveData : CardSaveData
    {
        public ArtefactSaveData (int id, bool isOpen, float itemExp, int level = 1) : base(id, isOpen, itemExp, level)
        {
        }
    }
}