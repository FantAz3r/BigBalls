using BigBalls.UI;

public class ItemUpgradeButton : ButtonClickHandler
{
    private ICardModel _cardModel;
    public void Init(ICardModel card) => _cardModel = card;

    protected override void OnClick()
    {
        _cardModel.UpgradeNoneGameLevel();
    }
}
