using BigBalls.GameplayObjects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigBalls.UI
{
    public class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private Slider _healthImage;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private float _smoothSpeed = 10f;

        private Tween _healthTween;
        private IReadonlyStat _health;

        public void Init(IReadonlyStat health)
        {
            _health = health;
        }

        private void Start()
        {
            if (_health == null)
                return;

            gameObject.SetActive(true);
            _healthImage.value = _health.CurrentValue / _health.MaxValue;
            _healthText.text = $"{_health.CurrentValue} / {_health.MaxValue}";
            _health.ValueChanged += View;
        }

        private void OnEnable()
        {
            if (_health == null)
                return;

            View(_health);
        }

        private void OnDestroy()
        {
            _health.ValueChanged -= View;
        }

        private void View(IReadonlyStat stat)
        {
            _healthText.text = $"{stat.CurrentValue:F0} / {stat.MaxValue:F0}";

            float startValue = _healthImage.value;
            float targetValue = stat.CurrentValue / stat.MaxValue;
            float duration = _smoothSpeed;

            HealthBarAnimation(startValue, targetValue, duration);
        }

        private void HealthBarAnimation(float startValue, float targetValue, float duration)
        {
            _healthTween?.Kill();
            _healthImage.value = startValue;
            _healthTween = DOTween.To(() => _healthImage.value, x => _healthImage.value = x, targetValue, duration)
                                 .SetEase(Ease.Linear)
                                 .OnComplete(() => _healthTween = null);
        }
    }
}
