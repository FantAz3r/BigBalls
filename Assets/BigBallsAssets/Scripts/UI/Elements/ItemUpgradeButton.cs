using BigBalls.UI;
using UnityEngine;

public class ItemUpgradeButton : ButtonClickHandler
{
    private ICardModel _cardModel;
<<<<<<< HEAD
    public void Init(ICardModel card) => _cardModel = card;

    protected override void OnClick()
    {
=======
    public void Init (ICardModel card) => _cardModel = card;

    protected override void OnClick ()
    {
        Debug.Log("gkirgjnozrg");
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
        _cardModel.UpgradeNoneGameLevel();
    }
}
