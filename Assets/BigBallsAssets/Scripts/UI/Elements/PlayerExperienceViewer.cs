using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerExperienceViewer : MonoBehaviour
{
    [SerializeField] private Slider _experienceSlider;
    [SerializeField] private TMP_Text _experienceText;
    [SerializeField] private float _smoothSpeed = 10f;

    private Tween _experienceTween;
    private IPlayerExperience _experience;

    public void Init (IPlayerExperience experience)
    {
        _experience = experience;
    }

    private void Start ()
    {
        if (_experience == null)
            return;

        gameObject.SetActive(true);

        _experienceSlider.minValue = 0f;
        _experienceSlider.maxValue = _experience.EXPForNextLevel;
        _experienceSlider.value = _experience.CurrentEXP;

        _experienceText.text =
            $"{_experience.CurrentEXP:F0} / {_experience.EXPForNextLevel:F0}";

        _experience.ValueChanged += View;
    }

    private void OnEnable ()
    {
        if (_experience == null)
            return;

        UpdateSliderMaxValue();
        View();
    }

    private void OnDestroy ()
    {
        if (_experience != null)
            _experience.ValueChanged -= View;

        _experienceTween?.Kill();
    }

    private void View ()
    {
        UpdateSliderMaxValue();

        _experienceText.text = $"{_experience.CurrentEXP:F0} / {_experience.EXPForNextLevel:F0}";

        float startValue = _experienceSlider.value;
        float targetValue = _experience.CurrentEXP;
        float duration = _smoothSpeed;

        ExperienceBarAnimation(startValue, targetValue, duration);
    }

    private void UpdateSliderMaxValue ()
    {
        _experienceSlider.maxValue = _experience.EXPForNextLevel;
    }

    private void ExperienceBarAnimation (
        float startValue,
        float targetValue,
        float duration)
    {
        _experienceTween?.Kill();

        _experienceSlider.value = startValue;

        _experienceTween = DOTween.To(
                () => _experienceSlider.value,
                value => _experienceSlider.value = value,
                targetValue,
                duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => _experienceTween = null);
    }
}
