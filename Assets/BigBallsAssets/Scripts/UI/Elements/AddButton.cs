using System;
using BigBalls.Services;
using BigBalls.UI;
using VContainer;

public class AddButton : ButtonClickHandler
{
    private IADVServise _aDVServise;
    private string _cardReward;
    private Action _action;

    [Inject]
    public void Construct (IADVServise aDVServise) => _aDVServise = aDVServise;

    public void SetRewardAction (Action action, string rewaedID)
    {
        _action = action;
        _cardReward = rewaedID;

        if (_aDVServise.CanShowRewardADV(rewaedID) == false)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    protected override void OnClick ()
    {
        if(_action != null)
            _aDVServise.TryShowRewardADV(_cardReward, _action);
    }
}
