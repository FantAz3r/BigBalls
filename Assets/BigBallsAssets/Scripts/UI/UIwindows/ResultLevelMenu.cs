using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.Infrastructure;
using BigBalls.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace BigBalls.UI
{
    public abstract class LevelResultMenu : PauseWindow
    {
        [Header("Chest")]
        [SerializeField] private Image _chestImage;

        [Header("Stars")]
        [SerializeField] private List<Image> _stars;

        [Header("Stats")]
        [SerializeField] private TMP_Text _kills;
        [SerializeField] private TMP_Text _damage;

        [Header("Sound")]
        [SerializeField] private SoundStruct _resultSound;

        protected IUIFactory UIFactory { get; private set; }
        protected DamageCalculator DamageCalculator { get; private set; }
        protected IEnemyFactory EnemyFactory { get; private set; }
        protected SoundSpawner SoundSpawner { get; private set; }

        private ChestData _chestData;

        [Inject]
        public void ConstructBase (
            IResourceLoader resourceLoader,
            IUIFactory uiFactory,
            DamageCalculator damageCalculator,
            IEnemyFactory enemyFactory,
            SoundSpawner soundSpawner)
        {
            UIFactory = uiFactory;
            DamageCalculator = damageCalculator;
            EnemyFactory = enemyFactory;
            SoundSpawner = soundSpawner;
            _chestData = resourceLoader.Load<ChestData>();
        }

        protected void ApplyResult (LevelID level, int compliteWaves, LevelButton levelButton)
        {
            SetupChest(level, compliteWaves, levelButton);
            RenderStars(compliteWaves);
            PlayResultSound();
            OnResultApplied(level, compliteWaves, levelButton);
            levelButton.gameObject.SetActive(false);
        }

        private void SetupChest (LevelID level, int compliteWaves, LevelButton levelButton)
        {
            ChestConfig chestConfig = _chestData.GetChest((Rarity) compliteWaves);

            if (_chestImage)
                _chestImage.sprite = chestConfig.CloseIcon;

            ChestModel chestModel = new ChestModel((int) level, chestConfig);
            ChestItemDropView chestItemDrop = UIFactory.Get<ChestItemDropView>(WindowType.OpenChest);

            UIFactory.Get<HUD>(WindowType.HUD).Close();
            chestItemDrop.Init(chestModel, levelButton);
            chestItemDrop.Close();
        }

        protected virtual void OnResultApplied (LevelID level, int compliteWaves, LevelButton levelButton) { }

        public override void Open ()
        {
            base.Open();
            RefreshStats();
        }

        protected void RefreshStats ()
        {
            if (_damage)
                _damage.text = ((int) DamageCalculator.GetDamage()).ToString();

            if (_kills)
                _kills.text = (EnemyFactory.DiedCount - EnemyFactory.SuisidedCount).ToString();
        }
        private void RenderStars (int compliteWaves)
        {
            for (int i = 0; i < _stars.Count; i++)
                _stars[i].gameObject.SetActive(i < compliteWaves);
        }

        private void PlayResultSound ()
        {
            if (SoundSpawner != null)
                SoundSpawner.Spawn(_resultSound);
        }
    }
}