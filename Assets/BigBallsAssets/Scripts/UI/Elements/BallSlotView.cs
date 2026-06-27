using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallSlotView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _text;

    public ICardModel Card { get; private set; }
    public void View(ICardModel card)
    {
        Card = card;
        _image.sprite = card.Config.Icon;
        _text.text = card.Level.ToString();
    }
}
