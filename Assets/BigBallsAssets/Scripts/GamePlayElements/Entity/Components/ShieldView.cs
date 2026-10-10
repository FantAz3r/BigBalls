using BigBalls.GameplayObjects;
using DG.Tweening; 
using UnityEngine;

public class ShieldView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    private Stat _shield;
    private Vector3 _startScale;
    private Tween _scaleTween;

    public void Init (Stat shield)
    {
        _shield = shield;
        _shield.ValueChanged += View;

        _startScale = transform.localScale;
        transform.localScale = Vector3.zero;
        _renderer.enabled = false;

        View(_shield);
    }

    private void OnDisable ()
    {
        if (_shield != null)
        {
            _shield.ValueChanged -= View;
        }

        _scaleTween?.Kill();
    }

    private void View (IReadonlyStat stat)
    {
        _scaleTween?.Kill();

        if (stat.CurrentValue > 0)
        {
            ShowShield();
        }
        else
        {
            HideShield();
        }
    }

    private void ShowShield ()
    {
        _renderer.enabled = true;

        _scaleTween = transform.DOScale(_startScale, 0.3f)
            .SetEase(Ease.OutBack);
    }

    private void HideShield ()
    {
        _scaleTween = transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                _renderer.enabled = false;
            });
    }
}
