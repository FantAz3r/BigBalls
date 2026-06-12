using System;
using System.Collections.Generic;
using BigBalls.Configs;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace BigBalls.UI
{
    public class WaveViewer : WindowBase
    {
        [SerializeField] private Slider _waveSlider;
        [SerializeField] private RectTransform _flagsContainer;
        [SerializeField] private WaveFlag _flagPrefab;

        private LevelConfig _levelConfig;
        private List<Wave> _waves;
        private List<float> _waveDurationsAccumulated = new List<float>();
        private List<WaveFlag> _flags = new List<WaveFlag>();
        private float _totalDuration;
        private LevelTimeline _levelTimeline;

        [Inject]
        public void Construct(LevelTimeline levelTimeline)
        {
            _levelTimeline = levelTimeline;
        }

        public void StartView(LevelConfig levelConfig)
        {
            _levelConfig = levelConfig;
            _waves = levelConfig.Waves;

            if (_waves == null || _waves.Count == 0)
                throw new ArgumentNullException();

            _totalDuration = levelConfig.GetLevelTime();

            for (int i = 0; i < _waves.Count - 1; i++)
            {
                _waveDurationsAccumulated.Add(levelConfig.GetWaveTime(i));
                Debug.Log(levelConfig.GetWaveTime(i));
            }

            _waveSlider.minValue = 0;
            _waveSlider.value = 0;
            _waveSlider.maxValue = _totalDuration;

            DrawFlags();
        }

        private void OnEnable()
        {
            _levelTimeline.TimeElapsed += View;
            _levelTimeline.NewWaveStarted += OnNewWaveStarted;
        }

        private void OnDisable()
        {
            _levelTimeline.TimeElapsed -= View;
            _levelTimeline.NewWaveStarted -= OnNewWaveStarted;
        }

        private void DrawFlags()
        {
            foreach (Transform child in _flagsContainer)
            {
                Destroy(child.gameObject);
            }

            _flags.Clear();

            float containerWidth = _flagsContainer.rect.width;

            for (int i = 0; i < _waves.Count; i++)
            {
                WaveFlag flag = Instantiate(_flagPrefab, _flagsContainer);
                var layoutElement = flag.GetComponent<LayoutElement>();
                if (layoutElement != null)
                    layoutElement.flexibleWidth = _levelConfig.GetWaveTime(i);

                _flags.Add(flag);
            }
        }

        private void OnNewWaveStarted(int waveNumber)
        {
            if (waveNumber <= 0 || waveNumber > _flags.Count)
                return;

            _flags[waveNumber-1].ActivateEffect();
        }

        private void View(float time)
        {
            if(_totalDuration == 0)
                return;

            if (time == _totalDuration)
                _flags[_flags.Count-1].ActivateEffect();


            if (time < _totalDuration)
            {
                _waveSlider.value = time;
            }
            else
            {
                _waveSlider.value = _totalDuration;
            }
        }
    }
}