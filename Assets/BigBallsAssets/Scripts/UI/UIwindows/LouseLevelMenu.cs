using BigBalls.Configs;
using UnityEngine;

namespace BigBalls.UI
{
    public class LouseLevelMenu : ResultLevelMenu
    {
        [SerializeField] private LevelButton _levelButton;

        public void LevelFailed (LevelConfig levelConfig, int compliteWaves)
        {
            ApplyResult(levelConfig, compliteWaves, _levelButton);
            _levelButton.Set(levelConfig.Level);
        }
    }
}