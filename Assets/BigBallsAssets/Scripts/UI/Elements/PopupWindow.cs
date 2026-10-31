using BigBalls.UI;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class PopupWindow : WindowBase
{
    private const float ViewDuration = 0.5f;
    private const float CloseDuration = 0.25f;
    private const float TargetScale = 1f;

    private Tween _scaleTween;
    private SoundSpawner _soundSpawner;

    [Inject]
    public void Construct (SoundSpawner soundSpawner) => _soundSpawner = soundSpawner;

    public override void Open ()
    {
        _scaleTween?.Kill();
        transform.localScale = Vector3.one * TargetScale;
        base.Open();
        transform.localScale = Vector3.zero;
        _soundSpawner.Spawn(SoundType.OpenPanel);

        _scaleTween = transform.DOScale(TargetScale, ViewDuration)
            .SetEase(Ease.OutBounce)
            .SetUpdate(true);
    }

    public override void Close ()
    {
        _scaleTween?.Kill();
        _soundSpawner.Spawn(SoundType.ClosePanel);

        _scaleTween = transform.DOScale(Vector3.zero, CloseDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => base.Close())
            .OnKill(() => base.Close());
    }
}
