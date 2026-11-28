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
        _ballPanel.Disable();

        foreach (Slot slot in _ballSlots)
            slot.Enable();

        foreach (Slot slot in _artefactSlots)
            slot.Disable();
    }

    private void OnEnable ()
    {
        if (_cardHolder == null)
            return;

        Refresh();
    }

    public void Init (PlayerCardHolder playerCardHolder)
    {
        if (_cardHolder != null)
        {
            _cardHolder.CardAdded -= OnCardAdded;
            _cardHolder.CardRemoved -= OnCardRemoved;
            _cardHolder.CardChanged -= OnCardChanged;
        }

        _cardHolder = playerCardHolder;

        _cardHolder.CardAdded += OnCardAdded;
        _cardHolder.CardRemoved += OnCardRemoved;
        _cardHolder.CardChanged += OnCardChanged;

        Refresh();
    }

    private void OnDestroy ()
    {
        if (_cardHolder == null)
            return;

        _cardHolder.CardAdded -= OnCardAdded;
        _cardHolder.CardRemoved -= OnCardRemoved;
        _cardHolder.CardChanged -= OnCardChanged;
    }


    private void OnCardAdded (ICardModel card) => AddCard(card);

    private void OnCardRemoved (ICardModel card) => RemoveCard(card);

    private void OnCardChanged (ICardModel card)
    {
        var slot = FindSlot(card);

        if (slot != null)
            slot.View(card);
    }


    private void Refresh ()
    {
        foreach (var slot in _ballSlots)
            slot.Clear();

        foreach (var slot in _artefactSlots)
            slot.Clear();

        _ballPanel.Disable();

        if (_cardHolder == null) return;

        foreach (var card in _cardHolder.Cards)
            AddCard(card);
    }

    // ---------- ADD / REMOVE ----------

    private void AddCard (ICardModel card)
    {
        if (card == null)
            return;

        if (card is BallModel)
        {
            var slot = _ballSlots.Find(s => s.Card == null);
            if (slot == null)
                return;

            _ballPanel.Enable();
            slot.Enable();
            slot.View(card);
        }
        else if (card is ArtefactModel)
        {
            var slot = _artefactSlots.Find(s => s.Card == null);
            if (slot == null)
                return;

            slot.Enable();
            slot.View(card);
        }
    }

    private void RemoveCard (ICardModel card)
    {
        if (card == null)
            return;

        var slot = FindSlot(card);

        if (slot == null)
            return;

        slot.Clear();

        UpdatePanelVisibility();
    }

    private Slot FindSlot (ICardModel card)
    {
        if (card is BallModel)
            return _ballSlots.Find(s => s.Card == card);

        if (card is ArtefactModel)
            return _artefactSlots.Find(s => s.Card == card);

        return null;
    }

    private void UpdatePanelVisibility ()
    {
        bool hasBalls = _ballSlots.Exists(s => s.Card != null);
        bool hasArtefacts = _artefactSlots.Exists(s => s.Card != null);

        _ballPanel.gameObject.SetActive(hasBalls);
        _artefactPanel.gameObject.SetActive(hasArtefacts);
    }
}