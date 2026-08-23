using BigBalls.UI;
using System;

public class ItemUpgradeButton : ButtonClickHandler
{
    private ICardModel _cardModel;
    public void Init(ICardModel card) => _cardModel = card;

    public event Action Clicked;
    protected override void OnClick()
    {
        Clicked?.Invoke();
    }
}
