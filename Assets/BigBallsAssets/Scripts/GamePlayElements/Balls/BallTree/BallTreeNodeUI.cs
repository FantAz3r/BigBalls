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

    public BallType BallType { get; private set; }
    private BallModel _node;
    private BallTreeController _controller;
    public RectTransform RectTransform => _rectTransform;

    public event Action<BallType> OnClick;


    private void OnEnable ()
    {
        UpdateState();
    }

    public void Init (BallType type, BallModel node, BallTreeController controller)
    {
        BallType = type;
        _node = node;
        _controller = controller;

        // _icon.sprite = GetSpriteForBallType(type);

        if (_unlockButton != null)
            _unlockButton.onClick.AddListener(Unlock);

        UpdateState();
    }

    public void UpdateState (BallModel node = null)
    {
        if (_controller == null)
            return;

        if(node != null)
        {
            _node = node;
        }

        bool isOpen = _node.IsOpen;
        bool canUnlock = _controller.CanUnlockBall(BallType);
        int level = _node.Level;

        _lockIcon.SetActive(isOpen == false);
        _unlockIcon.SetActive(isOpen);
        _levelText.text = isOpen ? $"Lv.{level}" : "Locked";

        if (isOpen == false)
        {
            _background.color = canUnlock ? _availableColor : _lockedColor;
            _priceText.text = canUnlock ? $"{_node.BallConfig.UnlockPrice} EXP" : "Locked";
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
        OnClick?.Invoke(BallType);
    }

    private void Unlock()
    {
        UpdateState();
    }
}
