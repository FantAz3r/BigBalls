using BigBalls.Infrastructure;
using BigBalls.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PlayButton : ButtonClickHandler
{
    private CreateLevelState _state;

    [Inject]
    public void Construct(CreateLevelState createLevelState)
    {
        _state = createLevelState;
    }

    protected override void OnClick()
    {
        base.OnClick();
        _state.StartLevel();
    }
}
