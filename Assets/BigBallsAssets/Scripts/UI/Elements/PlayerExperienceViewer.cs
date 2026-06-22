using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperienceViewer : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _valueText;

    private IPlayerExperience _experience;

    public void Init (IPlayerExperience experience)
    {
        _experience = experience;
        _slider.maxValue = experience.MaxValue;

        OnValueChanged(_experience);

        _experience.ValueChanged += OnValueChanged;
    }

    private void OnEnable ()
    {
        if (_experience != null)
        {
            _experience.ValueChanged += OnValueChanged;
        }
    }

    private void OnDisable ()
    {
        _experience.ValueChanged -= OnValueChanged;
    }

    private void OnValueChanged (IPlayerExperience experience)
    {
        _slider.value = experience.CurrentValue;
        _valueText.text = $"{experience.CurrentValue} / {experience.MaxValue}";
    }
}
