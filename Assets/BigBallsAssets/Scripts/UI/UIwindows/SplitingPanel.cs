using System.Collections.Generic;
using BigBalls.Services;
using BigBalls.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SplitingPanel : PauseWindow
{
    [SerializeField] private TMP_Text _mergeButtonText;
    [SerializeField] private TMP_Text _splitButtonText;
    [SerializeField] private Button _mergeButton;
    [SerializeField] private Button _splitButton;

    private IUIFactory _windowService;
    private IMergeService _mergeService;

    [field: SerializeField] public MergeWindow MergeWindow { get; private set; }
    [field: SerializeField] public SplitingWindow SplitingWindow { get; private set; }
    [field: SerializeField] public RectTransform ButtonPanel { get; private set; }

    [Inject]
    public void Construct(IUIFactory windowService, IMergeService mergeService)
    {
        _windowService = windowService;
        _mergeService = mergeService;
    }

    public override void Open()
    {
        ButtonPanel.gameObject.SetActive(true);
        SplitingWindow.gameObject.SetActive(false);
        MergeWindow.gameObject.SetActive(false);
        _windowService.Get<HUD>(WindowType.HUD).Close();

        base.Open();
        int mergeCount = _mergeService.GetAvailableMerges().Count;

        if (mergeCount > 0)
        {
            _mergeButton.gameObject.SetActive(true);
            _mergeButtonText.text = $"Merge: {mergeCount}";
        }
        else
        {
            _mergeButton.gameObject.SetActive(false);
        }

        if(SplitingWindow.CalculateCardsCount(out int minCount, out int maxCount, out List<ICardModel> cards) == false)
        {
            _splitButtonText.text = $"Coins: {SplitingWindow.MinGoldReward} - {SplitingWindow.MaxGoldReward}";
        }
        else
        {
            _splitButtonText.text = $"Cards: {minCount} - {maxCount}";
        }
    }
}
