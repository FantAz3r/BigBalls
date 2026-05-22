using BigBalls.GameplayObjects;
using BigBalls.Providers;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace BigBalls.UI
{
    public class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private Slider _healthImage;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private float _smoothSpeed = 10f;

        private Tween _healthTween;
        private Stat _health;


        [Inject]
        public void Construct(IPlayerProvider playerProvider)
        {
            _health = playerProvider.Stats.Where(stat => stat.Type == StatType.Health).FirstOrDefault();
        }

        private void Start()
        {
            gameObject.SetActive(true);
            _healthImage.value = _health.CurrentValue / _health.MaxValue;
            _healthText.text = $"{_health.CurrentValue} / {_health.MaxValue}";
            _health.ValueChanged += View;
        }

        private void OnEnable()
        {
            if (_health == null)
                return;

            View(_health.CurrentValue, _health.MaxValue);
        }

        private void OnDestroy()
        {
            _health.ValueChanged -= View;
        }

        private void View(float currentHealth, float maxHealth)
        {
            _healthText.text = $"{currentHealth:F0} / {maxHealth:F0}";

            float startValue = _healthImage.value;
            float targetValue = currentHealth / maxHealth;
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
