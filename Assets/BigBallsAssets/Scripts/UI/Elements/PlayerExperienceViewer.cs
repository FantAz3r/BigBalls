using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperienceViewer : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _valueText;
    
    private IPlayerExperience _experience;

    public void Init(IPlayerExperience experience)
    {
        _experience = experience;
        _slider.maxValue = experience.MaxValue;
        
        HandlerOnOnValueChanged(_experience);

        _experience.OnValueChanged += HandlerOnOnValueChanged;
    }

    private void OnDisable()
    {
        _experience.OnValueChanged -= HandlerOnOnValueChanged;
    }

    private void HandlerOnOnValueChanged(IPlayerExperience experience)
    {
        _slider.value = experience.CurrentValue;
        _valueText.text = $"{experience.CurrentValue} / {experience.MaxValue}";
    }
}
