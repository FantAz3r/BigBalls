using TMPro;
using UnityEngine;
using VContainer;

public class PlayerLevelViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;
    
    private IPlayerExperience _experience;
    private int _currentValue = 0;

    [Inject]
    public void Initialize(IPlayerExperience experience)
    {
        _experience = experience;
        _experience.LevelUpped += CreateNewLevelView;
    }

    private void Awake()
    {
        ShowValue();
    }

    private void OnDisable()
    {
        _experience.LevelUpped -= CreateNewLevelView;
    }

    private void CreateNewLevelView()
    {
        _currentValue++;
        ShowValue();
    }

    private void ShowValue()
    {
        _valueText.text = _currentValue.ToString();
    }
}
