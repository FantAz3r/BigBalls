using System;
using BigBalls.StaticData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallTreeNodeUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private GameObject _unlockIcon;
    [SerializeField] private Button _unlockButton;
    [SerializeField] private Color _lockedColor = Color.gray;
    [SerializeField] private Color _unlockedColor = Color.white;
    [SerializeField] private Color _availableColor = Color.green;

    private BallModel _node;
    private IBallUnlockService _unlockService;

    public event Action<BallType> OnClick;

    public BallType BallType { get; private set; }
    public RectTransform RectTransform => _rectTransform;

    private void OnEnable ()
    {
        UpdateState();
        _unlockButton?.onClick.AddListener(Unlock);
    }

    private void OnDisable ()
    {
        _unlockButton?.onClick.RemoveListener(Unlock);
    }

    public void Init (BallType type, BallModel node, IBallUnlockService unlockService)
    {
        BallType = type;
        _node = node;
        _unlockService = unlockService;
        _icon.sprite = node.Config.Icon;
        UpdateState();
    }

    public void UpdateState (BallModel node = null)
    {
        if (node != null)
            _node = node;

        bool isOpen = _node.IsOpen;
        bool canUnlock = _unlockService.CanUnlock(BallType);
        int level = _node.Level;

        _lockIcon.SetActive(isOpen == false);
        _unlockIcon.SetActive(isOpen);
        _levelText.text = isOpen ? $"Lv.{level}" : "Locked";

        if (isOpen == false)
        {
            _background.color = canUnlock ? _availableColor : _lockedColor;
            _priceText.text = canUnlock ? $"{_node.BallConfig.UnlockPrice} EXP" : "Locked";
            _priceText.gameObject.SetActive(true);
        }
        else
        {
            _background.color = _unlockedColor;
            _priceText.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        OnClick?.Invoke(BallType);
    }

    private void Unlock ()
    {
        UpdateState();
    }
}
