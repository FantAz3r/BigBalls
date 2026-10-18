using System.Collections.Generic;
using BigBalls.GameplayObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatTextHolder : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _currentValue;
    [SerializeField] private TMP_Text _nextLevelValue;
    [SerializeField] private Image _arrow;

    private void OnEnable ()
    {
        transform.localScale = Vector3.one;
    }

    public void RenderText (List<string> texts, bool showNextLevel = true)
    {
        _name.text = texts[0];
        _currentValue.text = FormatValue(texts[1]);

        if (texts.Count >= 3 && showNextLevel && texts[2] != "0")
        {
            _nextLevelValue.gameObject.SetActive(true);
            _nextLevelValue.text = FormatValue(texts[2]);
            _arrow.gameObject.SetActive(true);
        }
        else
        {
            _nextLevelValue.gameObject.SetActive(false);
            _arrow.gameObject.SetActive(false);

        }
    }

    public void RenderText (ItemStat itemStat, bool showNextLevel = true)
    {
        _name.text = itemStat.Name;
        _currentValue.text = itemStat.Value.ToString("0.##");

        if (itemStat.NextValue != 0 && showNextLevel)
        {
            _nextLevelValue.gameObject.SetActive(true);
            _nextLevelValue.text = itemStat.NextValue.ToString("0.##");
            _arrow.gameObject.SetActive(true);

        }
        else
        {
            _nextLevelValue.gameObject.SetActive(false);
            _arrow.gameObject.SetActive(false);

        }
    }

    private string FormatValue (string value)
    {
        if (float.TryParse(value, out float result))
        {
            return result.ToString("0.##");
        }

        return value;
    }
}
