using System.Collections.Generic;
using UnityEngine;

public class PlayerSlots : MonoBehaviour
{
    [SerializeField] private List<Slot> _ballSlots = new();
    [SerializeField] private List<Slot> _artefactSlots = new();

    [SerializeField] private RectTransform _ballPanel;
    [SerializeField] private RectTransform _artefactPanel;
    private PlayerCardHolder _cardHolder;


    private void Awake ()
    {
        _ballPanel.gameObject.SetActive(false);
        _artefactPanel.gameObject.SetActive(false);

        foreach (Slot slot in _ballSlots)
        {
            slot.gameObject.SetActive(false);
        }

        foreach (Slot slot in _artefactSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void Init(PlayerCardHolder playerCardHolder)
    {
        _cardHolder = playerCardHolder;

        foreach(var card in _cardHolder.Cards)
        {
            ViewSlot(card);
        }

        _cardHolder.CardChanged += ViewSlot;
    }

    private void ViewSlot(ICardModel card)
    {
        if (card is BallModel)
        {
            foreach (var slot in _ballSlots)
            {
                if (slot.Card == card || slot.Card == null)
                {
                    _ballPanel.gameObject.SetActive(true);
                    slot.gameObject.SetActive(true);
                    slot.View(card);
                    return;
                }
            }
        }
        else if(card is ArtefactModel)
        {
            foreach (var slot in _artefactSlots)
            {
                // if (slot.Card == card || slot.Card == null)
                // {
                //     _artefactPanel.gameObject.SetActive(true);
                //     slot.gameObject.SetActive(true);
                //     slot.View(card);
                //     return;
                // }
            }
        }
    }
}
