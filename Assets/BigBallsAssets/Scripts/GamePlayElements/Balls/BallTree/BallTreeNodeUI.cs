using System;
using System.Collections.Generic;
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
    [SerializeField] private Slider _slider;


    private List<BallTreeConnectionUI> _childConnections = new List<BallTreeConnectionUI>();
    private List<BallTreeConnectionUI> _parentConnections = new List<BallTreeConnectionUI>();
    private bool _isOpen;
    private BallModel _node;
    private IBallUnlockService _unlockService;

    public event Action<BallTreeNodeUI> OnClick;

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

        _slider.maxValue = _node.EXPForNextLevel;
        _slider.minValue = 0;
        _slider.value = _node.ItemEXP;

        _isOpen = _node.IsOpen;
        bool canUnlock = _unlockService.CanUnlock(BallType);
        int level = _node.Level;

        _lockIcon.SetActive(_isOpen == false);
        _unlockIcon.SetActive(_isOpen);
        _levelText.text = level.ToString();

        if (_isOpen == false)
        {
            _background.color = canUnlock ? _availableColor : _lockedColor;
            _priceText.text = canUnlock ? $"{_node.BallConfig.UnlockPrice} EXP" : "Locked";
            _priceText.gameObject.SetActive(true);
            DisableConnections();
            TryUpgradeConnaction();
        }
        else
        {
            _background.color = _unlockedColor;
            _priceText.gameObject.SetActive(false);

            foreach (var connection in _parentConnections)
            {
                connection.UpgradeConnaction();
            }

            UpdateConnections();
        }
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }

    public void Upgrade ()
    {
        _node.UpgradeNoneGameLevel();
        UpdateState();
    }

    public void AddChildConnection (BallTreeConnectionUI connectionUI) => _childConnections.Add(connectionUI);
    public void AddParentConnection (BallTreeConnectionUI connectionUI) => _parentConnections.Add(connectionUI);

    public void ClearConnections ()
    {
        _parentConnections.Clear();
        _childConnections.Clear();
    }

    public void UpdateConnections ()
    {
        foreach (var connection in _childConnections)
        {
            connection.UpdateLine();
        }
    }

    private void DisableConnections ()
    {
        foreach (var connection in _childConnections)
        {
            connection.DisableLine();
        }
    }

    private void TryUpgradeConnaction ()
    {
        int count = 0;

        foreach (var connection in _parentConnections)
        {
            if (connection.IsConnected)
            {
                count++;
            }
        }

        if (count >= _parentConnections.Count)
        {
            
        }
    }

    private void Unlock ()
    {
        if (_isOpen)
        {
            Upgrade();
        }
        else
        {
            UpdateState();
        }
    }
}
