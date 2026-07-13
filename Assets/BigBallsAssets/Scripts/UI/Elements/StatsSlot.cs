using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsSlot : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _level;

    public void RenderSlots(ICardModel cardModel, ICardModel itemModel)
    {
        _image.sprite = cardModel.Config.Icon;
        _level.text = (cardModel.Level + itemModel.Level).ToString();
    }
}
