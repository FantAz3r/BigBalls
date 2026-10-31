using BigBalls.Infrastructure;
using TMPro;
using UnityEngine;

namespace BigBalls.UI
{
    public class WinLevelMenu : ResultLevelMenu
    {
        [SerializeField] private TMP_Text _compliteText;
        [SerializeField] private TMP_Text _levelStats;
        [SerializeField] private LevelButton _levelButton;

        public void LevelComplite (LevelID level, int compliteWaves)
        {
            ApplyResult(level, compliteWaves, _levelButton);
        }

        protected override void OnResultApplied (LevelID level, int compliteWaves, LevelButton levelButton)
        {
            levelButton.Set(level + 1);
        }
    }
}