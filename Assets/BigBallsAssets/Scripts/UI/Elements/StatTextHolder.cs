using BigBalls.GameplayObjects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatTextHolder : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _currentValue;
    [SerializeField] private TMP_Text _nextLevelValue;

    public void RenderText(List<string> texts, bool showNextLevel = true)
    {
        _name.text = texts[0];
        _currentValue.text = texts[1];

        if (texts.Count >= 2 && showNextLevel)
        {
            _nextLevelValue.gameObject.SetActive(true);
            _nextLevelValue.text = texts[2];
        }
        else
        {
            _nextLevelValue.gameObject.SetActive(false);
        }
    }

    public void RenderText(ItemStat itemStat, bool showNextLevel = true)
    {
        _name.text = itemStat.Name;
        _currentValue.text = itemStat.Value.ToString();

        if (showNextLevel)
        {
            _nextLevelValue.gameObject.SetActive(true);
            _nextLevelValue.text = itemStat.NextValue.ToString();
        }
        else
        {
            _nextLevelValue.gameObject.SetActive(false);
        }
    }
}
