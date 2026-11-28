using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.UI;
using UnityEngine;

public class TutorialPanel : ButtonClickHandler
{
    public event Action Clicked;

    protected override void OnClick ()
    {
        Clicked?.Invoke();
    }
}
