using System;
using BigBalls.Configs;
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

    public RectTransform RectTransform => _rectTransform;
    public event Action<BallType> OnClick;

    private BallType _ballType;
    private BallConfig _node;
    private BallTreeController _controller;

    public void Initialize (BallType type, BallConfig node, BallTreeController controller)
    {
        _ballType = type;
        _node = node;
        _controller = controller;

        // Настраиваем иконку
        // _icon.sprite = GetSpriteForBallType(type);

        if (_unlockButton != null)
            _unlockButton.onClick.AddListener(OnUnlockClick);

        UpdateState();
    }

    public void UpdateState ()
    {
        if (_controller == null)
            return;

        bool isOpen = _controller.IsBallOpen(_ballType);
        bool canUnlock = _controller.CanUnlockBall(_ballType);
        int level = _controller.GetBallLevel(_ballType);

        // Обновляем состояние
        _lockIcon.SetActive(isOpen == false);
        _unlockIcon.SetActive(isOpen);
        _levelText.text = isOpen ? $"Lv.{level}" : "Locked";

        if (isOpen == false)
        {
            _background.color = canUnlock ? _availableColor : _lockedColor;
            _priceText.text = canUnlock ? $"{_node.UnlockPrice} EXP" : "Locked";
            _priceText.gameObject.SetActive(true);

            //if (_unlockButton != null)
            //    _unlockButton.gameObject.SetActive(canUnlock);
        }
        else
        {
            _background.color = _unlockedColor;
            _priceText.gameObject.SetActive(false);

            //if (_unlockButton != null)
            //    _unlockButton.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        OnClick?.Invoke(_ballType);
    }

    private void OnUnlockClick ()
    {
        if (_controller.TryUnlockBall(_ballType))
        {
            UpdateState();
        }
    }
}
