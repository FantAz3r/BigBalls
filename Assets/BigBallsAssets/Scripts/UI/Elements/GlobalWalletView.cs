using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;

public class GlobalWalletView : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyText;

    private GlobalWallet _globalWallet;
    private SoundSpawner _soundSpawner;
    private Tween _rejectedTween;

    [Inject]
    public void Construct(GlobalWallet globalWallet, SoundSpawner soundSpawner)
    {
        _globalWallet = globalWallet;
        _soundSpawner = soundSpawner;
        _globalWallet.OnValueChange += View;
        _globalWallet.Rejected += OnPurchaseRejected;
        View();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(_globalWallet != null)
        {
            _globalWallet.OnValueChange -= View;
            _globalWallet.Rejected -= OnPurchaseRejected;
        }
    }

    private void View()
    {
        _moneyText.text = _globalWallet.Coins.ToString();
    }

    private void OnPurchaseRejected()
    {
        _soundSpawner.Spawn(SoundType.PurchaseRejected);
        PlayNotEnoughMoney();
    }

    private void PlayNotEnoughMoney ()
    {
        RectTransform rect = _moneyText.rectTransform;
        Vector2 originPos = rect.anchoredPosition;

        Color originColor = _moneyText.color;
        Color errorColor = new Color(1f, 0.25f, 0.25f, 1f);

        _rejectedTween?.Kill();

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        seq.Append(rect.DOPunchAnchorPos(new Vector2(10f, 0f), 0.3f, 10, 0.5f));
        seq.Join(_moneyText.DOColor(errorColor, 0.15f).SetLoops(2, LoopType.Yoyo));
        seq.OnComplete(() =>
        {
            _moneyText.color = originColor;
            rect.anchoredPosition = originPos;
        });

        _rejectedTween = seq;
    }
}
