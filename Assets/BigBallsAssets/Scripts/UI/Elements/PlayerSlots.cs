using System.Collections.Generic;
using UnityEngine;

public class PlayerSlots : MonoBehaviour
{
    [SerializeField] private List<Slot> _ballSlots = new();
    [SerializeField] private List<Slot> _artefactSlots = new();
    private PlayerCardHolder _cardHolder;

    public void Init(PlayerCardHolder playerCardHolder)
    {
        _cardHolder = playerCardHolder;
        _cardHolder.CardChanged += ViewSlot;
    }

    private void ViewSlot(ICardModel card)
    {
        if (card is BallModel)
        {
            foreach (var slot in _ballSlots)
            {
                if(slot.Card == card || slot.Card == null)
                {
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
               //     slot.View(card);
               //     return;
               // }
            }
        }
    }
}
