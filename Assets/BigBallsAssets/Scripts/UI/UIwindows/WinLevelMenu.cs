<<<<<<< HEAD
using BigBalls.Infrastructure;
using BigBalls.Services;
using System.Collections.Generic;
=======
using System.Collections.Generic;
using BigBalls.Infrastructure;
using BigBalls.Services;
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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

<<<<<<< HEAD
        private IUIFactory _windowService;
        private ChestData _chestData;

        [Inject]
        public void Construct(IResourceLoader resourceLoader, IUIFactory windowService)
        {
            _windowService = windowService;
            _chestData = resourceLoader.Load<ChestData>();
        }

        public void LevelComplite(LevelID level, int compliteWaves)
        {
            ChestConfig chestConfig = _chestData.GetChest((Rarity)compliteWaves);
            _chestImage.sprite = chestConfig.CloseIcon;
            ChestModel chestModel = new ChestModel((int)level, chestConfig);
            _windowService.Get<ChestItemDropView>(WindowType.OpenChest).Init(chestModel);
        }
=======
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

>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    }
}