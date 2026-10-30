using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;
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
    }

    protected override void OnClick ()
    {
        Debug.Log("reclama");
        _aDVServise.TryShowRewardADV(_cardReward, _action);
    }
}
