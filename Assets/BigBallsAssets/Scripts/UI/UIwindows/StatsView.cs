using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _stats;
    [SerializeField] private TMP_Text _level;

    private void Start () => gameObject.SetActive(false);

    public void View (ICardModel card)
    {
        gameObject.SetActive(true);
        _image.sprite = card.Config.Icon;
        _nameText.text = card.Name;
        _descriptionText.text = card.Description;
        _level.text = card.Level.ToString();
        InitStats(card);
    }

    private void InitStats (ICardModel card)
    {
        _stats.text = string.Empty;
    }
}