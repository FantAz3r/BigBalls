using System.Collections.Generic;
using BigBalls.Infrastructure;
using BigBalls.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace BigBalls.UI
{
    public class WinLevelMenu : WindowBase
    {
        [SerializeField] private Image _chestImage;
        [SerializeField] private TMP_Text _compliteText;
        [SerializeField] private List<Image> _stars;
        [SerializeField] private TMP_Text _levelStats;

        private ChestData _chestData;

        [Inject]
        public void Construct(IResourceLoader resourceLoader)
        {
            _chestData = resourceLoader.Load<ChestData>();
        }

        public void LevelComplite (LevelID level, int compliteWaves)
        {
            _chestImage.sprite = _chestData.GetChest((Rarity) compliteWaves).CloseIcon;
        }

    }
}