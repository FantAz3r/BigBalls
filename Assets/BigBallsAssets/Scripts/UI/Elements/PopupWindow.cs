using System.Collections;
using System.Collections.Generic;
using BigBalls.Services;
using BigBalls.UI;
using DG.Tweening;
using UnityEngine;

public class PopupWindow : WindowBase
{
    private const float ViewDuration = 0.5f;
    private const float CloseDuration = 0.25f;
    private const float TargetScale = 1f;

    private Tween _scaleTween;

    public override void Open ()
    {
        base.Open();

        _scaleTween?.Kill();
        transform.localScale = Vector3.zero;

        _scaleTween = transform.DOScale(TargetScale, ViewDuration)
            .SetEase(Ease.OutBounce)
            .SetUpdate(true);
    }

    public override void Close ()
    {
        _scaleTween?.Kill();

        _scaleTween = transform.DOScale(Vector3.zero, CloseDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                base.Close();
            });
    }
}
