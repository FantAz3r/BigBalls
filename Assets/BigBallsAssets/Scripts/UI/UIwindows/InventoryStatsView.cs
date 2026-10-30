using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Services;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class InventoryStatsView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _maxLevelText;
    [SerializeField] private ItemUpgradeButton _upgradeButton;
    [SerializeField] private RectTransform _statsParent;
    [SerializeField] private RectTransform _slotsParent;
    //[SerializeField] private Slider _expSlider;

    private ICardModel _cardModel;
    private StatTextHolder _statTextHolder;
    private StatsSlot _slotPrefab;
    private List<StatTextHolder> _statTextHolders = new();
    private List<StatsSlot> _statsSlots = new();
    private SoundSpawner _soundSpawner;
    //private Tween _notEnoughEXPTween;

    private void Start () => gameObject.SetActive(false);

    private void OnDestroy ()
    {
        _upgradeButton.Clicked -= Upgrade;
    }

    [Inject]
    public void Init (IResourceLoader resourceLoader, SoundSpawner soundSpawner)
    {
        _statTextHolder = resourceLoader.Load<StatTextHolder>();
        _soundSpawner = soundSpawner;
        _slotPrefab = resourceLoader.Load<StatsSlot>();
    }

    public void View (ICardModel card)
    {
        ClearStats();
        ClearSlots();

        _cardModel = card;

        gameObject.SetActive(true);

        _upgradeButton.Clicked -= Upgrade;
        _upgradeButton.Init(_cardModel);
        _upgradeButton.Clicked += Upgrade;

        if (_cardModel.NoneGameLevel == _cardModel.MaxNoneGameLevel)
        {
            _upgradeButton.gameObject.SetActive(false);
            _maxLevelText.gameObject.SetActive(true);
        }
        else
        {
            _upgradeButton.gameObject.SetActive(true);
            _maxLevelText.gameObject.SetActive(false);
        }

        _image.sprite = _cardModel.Config.Icon;
        _nameText.text = _cardModel.Name;
        _descriptionText.text = _cardModel.Description;
        _level.text = _cardModel.Level.ToString();
        RenderStats(card);
    }

    private void RenderStats (ICardModel card)
    {
        _slotsParent.gameObject.SetActive(false);
        _statsParent.gameObject.SetActive(false);

        if (card is WeaponModel weaponModel)
        {
            _slotsParent.gameObject.SetActive(true);

            foreach (var item in weaponModel.UniqueBallModels())
            {
                StatsSlot slot = Instantiate(_slotPrefab, _slotsParent);
                slot.RenderSlots(item, weaponModel);
                _statsSlots.Add(slot);
            }
        }
        else if (card is HelmetModel helmetModel)
        {
            _slotsParent.gameObject.SetActive(true);

            foreach (var item in helmetModel.GetArtefacts())
            {
                StatsSlot slot = Instantiate(_slotPrefab, _slotsParent);
                slot.RenderSlots(item, helmetModel);
                _statsSlots.Add(slot);
            }
        }
        else
        {
            _statsParent.gameObject.SetActive(true);

            foreach (var item in card.GetStatsText(true))
            {
                StatTextHolder statTextHolder = Instantiate(_statTextHolder, _statsParent);
                statTextHolder.RenderText(item);
                _statTextHolders.Add(statTextHolder);
            }
        }
    }

    private void ClearStats () => ClearCollection(_statTextHolders);

    private void ClearSlots () => ClearCollection(_statsSlots);

    private void Upgrade ()
    {
        if (_cardModel.TryUpgradeNoneGameLevel())
        {
            if (_cardModel.NoneGameLevel == _cardModel.MaxNoneGameLevel)
            {
                _upgradeButton.gameObject.SetActive(false);
                _maxLevelText.gameObject.SetActive(true);
            }
        }
        else
        {
            OnUpgradeFailed();
            _soundSpawner.Spawn(SoundType.Error);
            return;
        }

        _soundSpawner.Spawn(SoundType.Upgrade, _cardModel.NoneGameLevel - 1, _cardModel.MaxNoneGameLevel);
        View(_cardModel);
    }

    private void OnUpgradeFailed ()
    {
       // RectTransform sliderRect = _expSlider.transform as RectTransform;
       // Vector2 originPos = sliderRect.anchoredPosition;
       //
       // _notEnoughEXPTween?.Kill();
       //
       // _notEnoughEXPTween = sliderRect
       //     .DOPunchAnchorPos(new Vector2(10f, 0f), 0.3f, 10, 0.5f)
       //     .SetUpdate(true)
       //     .OnComplete(() => sliderRect.anchoredPosition = originPos);
    }

    private void ClearCollection<T> (List<T> collection) where T : MonoBehaviour
    {
        if (collection.Count == 0)
            return;

        foreach (var item in collection)
        {
            Destroy(item.gameObject);
        }

        collection.Clear();
    }
}