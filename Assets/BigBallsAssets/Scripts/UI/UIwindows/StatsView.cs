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
    [SerializeField] private ItemUpgradeButton _upgradeButton;

    private ICardModel _cardModel;

    private void Start () => gameObject.SetActive(false);

    private void OnDestroy () => _cardModel.Upgraded -= View;

    public void View (ICardModel card)
    {
        if(_cardModel != null)
        {
            _cardModel.Upgraded -= View;
        }
        _cardModel = card;
        _cardModel.Upgraded += View;

        gameObject.SetActive(true);
        _upgradeButton.Init(_cardModel);
        _image.sprite = _cardModel.Config.Icon;
        _nameText.text = _cardModel.Name;
        _descriptionText.text = _cardModel.Description;
        _level.text = _cardModel.Level.ToString();
        InitStats(_cardModel);
    }

    private void InitStats (ICardModel card)
    {
        _stats.text = string.Empty;
    }
}