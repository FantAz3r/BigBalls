using System;
using BigBalls.UI;
using UnityEngine;

[RequireComponent(typeof(HighliterUI))]
public class ItemUpgradeButton : ButtonClickHandler
{
    private ICardModel _cardModel;

    public event Action Clicked;
    public HighliterUI HighliterUI { get; private set; }
    public void Init (ICardModel card) => _cardModel = card;

    protected override void Awake ()
    {
        base.Awake ();
        HighliterUI = GetComponent<HighliterUI>();
    }

    protected override void OnClick ()
    {
        Clicked?.Invoke();
    }
}
