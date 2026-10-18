using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.UI;
using UnityEngine;
using VContainer;

public class MergeWindow : MonoBehaviour
{
    private readonly List<CardView> _spawned = new();

    [SerializeField] private CardView _slotPrefab;
    [SerializeField] private Transform _slotsRoot;

    private PlayerCardHolder _cardHolder;
    private BallRepository _ballRepository;
    private SplitingPanel _splitingPanel;
    private IWindowService _windowService;
    private IMergeService _mergeService;
    private IPoolService _poolService;
    private IUIFactory _uIFactory;

    [Inject]
    public void Construct (IMergeService mergeService, BallRepository ballRepository, IPoolService poolService, IUIFactory uIFactory, IWindowService windowService)
    {
        _mergeService = mergeService;
        _ballRepository = ballRepository;
        _poolService = poolService;
        _uIFactory = uIFactory;
        _windowService = windowService;
    }

    public void Init (PlayerCardHolder cardHolder)
    {
        _cardHolder = cardHolder;
        _splitingPanel = _uIFactory.Get<SplitingPanel>(WindowType.MergeWindow);
    }

    public void Open ()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public void Close ()
    {
        gameObject.SetActive(false);

        foreach (var card in _spawned)
        {
            _poolService.Release(card);
        }

        _spawned.Clear();
        _windowService.Open<HUD>();
        _splitingPanel.Close();
    }

    private void Refresh ()
    {
        foreach (var ball in _mergeService.GetAvailableMerges())
        {
            var slot = _poolService.Get(_slotPrefab, _slotsRoot);
            slot.Init(_cardHolder);
            slot.Render(ball);
            slot.Selected += OnMergeClicked;
            _spawned.Add(slot);
        }
    }

    private void OnMergeClicked (CardView target)
    {
        if (target.Card is not BallModel ballModel)
            return;

        Close();

        foreach (var parentType in ballModel.BallConfig.ParentTypes)
        {
            var parent = _ballRepository.AllModels[parentType];
            _cardHolder.Remove(parent);
        }

        target.Selected -= OnMergeClicked;
    }
}