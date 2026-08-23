using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatTextHolder : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _currentValue;
    [SerializeField] private TMP_Text _nextLevelValue;

    public void RenderText(List<string> texts)
    {
        _name.text = texts[0];
        _currentValue.text = texts[1];

        if (texts.Count >= 2)
        {
            _nextLevelValue.text = texts[2];
        }
        else
        {
            _nextLevelValue.gameObject.SetActive(false);
        }
    }

    public void RenderText(ItemStat itemStat)
    {
        _name.text = itemStat.Name;
        _currentValue.text = itemStat.Value.ToString();
        _nextLevelValue.text = itemStat.NextValue.ToString();
    }
}
