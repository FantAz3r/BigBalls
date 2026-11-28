using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Infrastructure;
using BigBalls.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace BigBalls.UI
{
    public abstract class ResultLevelMenu : PauseWindow
    {
        [Header("Chest")]
        [SerializeField] private Image _chestImage;
        [SerializeField] private OpenChestButton _chestButton;

        [Header("Stars")]
        [SerializeField] private List<Image> _stars;
        [SerializeField] private List<Image> _starsBlack;
        [SerializeField] private float _starViewDelay = 0.33f;

        [Header("Stats")]
        [SerializeField] private TMP_Text _kills;
        [SerializeField] private TMP_Text _damage;
        [SerializeField] private TMP_Text _time;
        [SerializeField] private TMP_Text _coins;
        [SerializeField] private TMP_Text _score;


        [Header("Sound")]
        [SerializeField] private SoundStruct _resultSound;

        protected IUIFactory UIFactory { get; private set; }
        protected DamageCalculator DamageCalculator { get; private set; }
        protected IEnemyFactory EnemyFactory { get; private set; }
        protected SoundSpawner SoundSpawner { get; private set; }
        protected ScoreCounter ScoreCounter  { get; private set; }
        protected IWalletModel WalletModel { get; private set; }
        protected LevelTimeCounter LevelTimeCounter { get; private set; }

        private ChestData _chestData;
        private WaitForSecondsRealtime _delay;

        [Inject]
        public void ConstructBase (
            IResourceLoader resourceLoader,
            IUIFactory uiFactory,
            DamageCalculator damageCalculator,
            IEnemyFactory enemyFactory,
            SoundSpawner soundSpawner,
            ScoreCounter scoreCounter,
            IWalletModel walletModel,
            LevelTimeCounter levelTimeCounter)
        {
            UIFactory = uiFactory;
            DamageCalculator = damageCalculator;
            EnemyFactory = enemyFactory;
            SoundSpawner = soundSpawner;
            ScoreCounter = scoreCounter;
            WalletModel = walletModel;
            LevelTimeCounter = levelTimeCounter;
            _chestData = resourceLoader.Load<ChestData>();
            _delay = new WaitForSecondsRealtime(_starViewDelay);
        }

        protected void ApplyResult (LevelConfig config, int compliteWaves, LevelButton levelButton)
        {
            Debug.Log(compliteWaves);
            SetupChest(config.Level, compliteWaves, levelButton);
            StartCoroutine(RenderStars(compliteWaves, config));
            PlayResultSound();
            OnResultApplied(config.Level, compliteWaves, levelButton);

            _chestButton.SetActive(compliteWaves > 0);
            levelButton.SetActive(compliteWaves > 0 == false);
        }

        private void SetupChest (LevelID level, int compliteWaves, LevelButton levelButton)
        {
            if (compliteWaves > 0)
            {
                levelButton.gameObject.SetActive(true);
                ChestConfig chestConfig = _chestData.GetChest((Rarity) compliteWaves);

                if (_chestImage)
                    _chestImage.sprite = chestConfig.CloseIcon;

                ChestModel chestModel = new ChestModel((int) level, chestConfig);
                ChestItemDropView chestItemDrop = UIFactory.Get<ChestItemDropView>(WindowType.OpenChest);

                UIFactory.Get<HUD>(WindowType.HUD).Close();
                chestItemDrop.Init(chestModel, levelButton);
                chestItemDrop.Close();
            }
        }

        protected virtual void OnResultApplied (LevelID level, int compliteWaves, LevelButton levelButton) { }

        public override void Open ()
        {
            base.Open();
            LevelTimeCounter.Stop();
            RefreshStats();
        }

        protected void RefreshStats ()
        {
            if (_damage)
                _damage.text = ((int) DamageCalculator.GetDamage()).ToString();

            if (_kills)
                _kills.text = (EnemyFactory.DiedCount - EnemyFactory.SuisidedCount).ToString();

            if (_coins)
                _coins.text = WalletModel.CurrentValue.ToString();

            if (_time)
                _time.text = LevelTimeCounter.ElapsedFormatted;

            if (_score)
                _score.text = ((int)ScoreCounter.CurrentScore).ToString();
        }

        private IEnumerator RenderStars (int compliteWaves, LevelConfig levelConfig)
        {
            foreach (var star in _starsBlack)
                star.Disable();

            for (int j = 0; j < levelConfig.GetBossCount(); j++)
                _starsBlack[j].Enable();


            for (int i = 0; i < _stars.Count; i++)
            {
                yield return _delay;
                _stars[i].gameObject.SetActive(i < compliteWaves);
            }
        }

        private void PlayResultSound ()
        {
            if (SoundSpawner != null)
                SoundSpawner.Spawn(_resultSound);
        }
    }
}