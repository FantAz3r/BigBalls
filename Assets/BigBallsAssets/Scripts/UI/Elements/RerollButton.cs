using BigBalls.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;

public class RerollButton : ButtonClickHandler
{
    [SerializeField] private CardSelectionMenu _cardMenu;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private int _rerollNumber = 1;
    [SerializeField] private int _startCost = 5;

    [Header("Shake Settings")]
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float _shakeStrength = 10f;
    [SerializeField] private int _shakeVibrato = 20;
    [SerializeField] private float _shakeRandomness = 90f;

    private int _currentCost;
    private IWalletModel _walletModel;
    private SoundSpawner _soundSpawner;
    private RectTransform _transform;

    [Inject]
    public void Construct (IWalletModel walletModel, SoundSpawner soundSpawner)
    {
        _walletModel = walletModel;
        _soundSpawner = soundSpawner;
    }

    private void Start ()
    {
        _transform = GetComponent<RectTransform>();
        //_currentCost = _startCost;
        _currentCost = 0;
        _costText.text = _currentCost.ToString();
    }

    protected override void OnClick ()
    {
        if (_walletModel.TrySpendCoins(_currentCost))
        {
            //_currentCost = _startCost * _rerollNumber + _currentCost;
            _costText.text = _currentCost.ToString();
            _rerollNumber++;
            _cardMenu.Reroll();
            _soundSpawner.Spawn(SoundType.ButtonClick);
        }
        else
        {
            ShakeCostText();
            _soundSpawner.Spawn(SoundType.Error);
        }
    }

    private void ShakeCostText ()
    {
        if (_costText == null)
            return;

        _transform.DOKill();
        _transform.DOShakeAnchorPos(
                _shakeDuration,
                new Vector2(_shakeStrength, 0f),
                _shakeVibrato,
                _shakeRandomness,
                snapping: false,
                fadeOut: true)
            .SetUpdate(true);
    }
}
