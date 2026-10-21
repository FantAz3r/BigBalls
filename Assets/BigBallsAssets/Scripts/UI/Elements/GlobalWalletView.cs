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
    private RectTransform _textTransform;

    private Vector2 _basePosition;
    private Color _baseColor;

    [Inject]
    public void Construct (GlobalWallet globalWallet, SoundSpawner soundSpawner)
    {
        _globalWallet = globalWallet;
        _soundSpawner = soundSpawner;
        _globalWallet.OnValueChange += View;
        _globalWallet.Rejected += OnPurchaseRejected;
        _textTransform = _moneyText.rectTransform;

        _basePosition = _textTransform.anchoredPosition;
        _baseColor = _moneyText.color;

        View();
    }

    public void Enable () => gameObject.SetActive(true);
    public void Disable () => gameObject.SetActive(false);

    private void OnDestroy ()
    {
        if (_globalWallet != null)
        {
            _globalWallet.OnValueChange -= View;
            _globalWallet.Rejected -= OnPurchaseRejected;
        }
        _rejectedTween?.Kill();
    }

    private void View ()
    {
        _moneyText.text = _globalWallet.Coins.ToString();
    }

    private void OnPurchaseRejected ()
    {
        _soundSpawner.Spawn(SoundType.PurchaseRejected);
        PlayNotEnoughMoney();
    }

    private void PlayNotEnoughMoney ()
    {
        Color errorColor = new Color(1f, 0.25f, 0.25f, 1f);

        _rejectedTween?.Kill();
        _textTransform.anchoredPosition = _basePosition;
        _moneyText.color = _baseColor;

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        seq.Append(_textTransform.DOPunchAnchorPos(new Vector2(10f, 0f), 0.3f, 10, 0.5f));
        seq.Join(_moneyText.DOColor(errorColor, 0.15f).SetLoops(2, LoopType.Yoyo));

        seq.OnComplete(() =>
        {
            _moneyText.color = _baseColor;
            _textTransform.anchoredPosition = _basePosition;
        });

        _rejectedTween = seq;
    }
}