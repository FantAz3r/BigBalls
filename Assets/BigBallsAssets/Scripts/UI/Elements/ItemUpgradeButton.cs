using BigBalls.UI;
using System;
using UnityEngine;

[RequireComponent(typeof(HighliterUI))]
public class ItemUpgradeButton : ButtonClickHandler
{
    private ICardModel _cardModel;

    public event Action Clicked;
    public HighliterUI HighliterUI { get; private set; }
    public void Init(ICardModel card) => _cardModel = card;

    private void Awake()
    {
        HighliterUI = GetComponent<HighliterUI>();
    }

    protected override void OnClick()
    {
        Clicked?.Invoke();
    }
}
