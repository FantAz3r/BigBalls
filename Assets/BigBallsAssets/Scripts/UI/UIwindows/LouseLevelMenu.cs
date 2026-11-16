using BigBalls.Infrastructure;
using UnityEngine;

namespace BigBalls.UI
{
    public class LouseLevelMenu : ResultLevelMenu
    {
        [SerializeField] private LevelButton _levelButton;

        public void LevelFailed (LevelID level, int compliteWaves)
        {
            ApplyResult(level, compliteWaves, _levelButton);
            _levelButton.Set(level);
        }
    }
}