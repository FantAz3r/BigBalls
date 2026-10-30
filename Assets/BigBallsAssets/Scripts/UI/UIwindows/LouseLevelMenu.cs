using BigBalls.GameplayObjects;
using BigBalls.Infrastructure;
using TMPro;
using UnityEngine;
using VContainer;

namespace BigBalls.UI
{
    public class LouseLevelMenu : LevelResultMenu
    {
        [SerializeField] private TMP_Text _time;
        [SerializeField] private TMP_Text _compliteLines;
        [SerializeField] private TMP_Text _collectedCoins;
        [SerializeField] private LevelButton _levelButton;

        private LevelTimeline _levelTimeline;
        private EnemySpawner _enemySpawner;

        [Inject]
        public void Construct (LevelTimeline levelTimeline, EnemySpawner enemySpawner)
        {
            _levelTimeline = levelTimeline;
            _enemySpawner = enemySpawner;
        }

        public void LevelFailed (LevelID level, int compliteWaves)
        {
            ApplyResult(level, compliteWaves, _levelButton);
            _levelButton.Set(level);
        }
    }
}