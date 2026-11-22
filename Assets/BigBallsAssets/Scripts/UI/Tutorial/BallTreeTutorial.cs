using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BallTreeTutorial : MonoBehaviour
{
    [SerializeField] private List<Vector3> _points = new List<Vector3>();

    [SerializeField] private RectTransform _tutorialPanel;
    [SerializeField] private BallTreeUI _tree;
    [SerializeField] private ClickHand _clickHand;

    [SerializeField] private HighliterUI _exit;

    [SerializeField] private RectTransform _frontPanel;
    [SerializeField] private RectTransform _backPanel;

    [SerializeField] private ItemUpgradeButton _upgradeButton;

    private ISaveService _saveService;

    [Inject]
    public void Construct(ISaveService saveService)
    {
        _saveService = saveService;
    }

    private void OnEnable()
    {
        if (_saveService == null)
            return;

        _tree.Inited += ClickBase;

    }

    private void OnDisable()
    {
        _tree.Inited -= ClickBase;
    }


    public void ClickBase()
    {
        if (_saveService.GameProgress.BallTreeTutotialComplite)
        {
            CloseTutorial();
            return;
        }

        _clickHand.PlayClick(_points[0]);
        BallTreeNodeUI nodeUI = _tree.Get(BallType.Base);
        nodeUI.OnClick += UpgradeBase;
        nodeUI.HighliterUI.Show(3);
    }

    public void UpgradeBase(BallTreeNodeUI nodeUI)
    {
        nodeUI.OnClick -= UpgradeBase;
        nodeUI.HighliterUI.Hide();

        _clickHand.PlayClick(_points[1], _frontPanel);
        _upgradeButton.HighliterUI.Show(3);
        _upgradeButton.Clicked += ClickFire;

    }

    public void ClickFire()
    {
        _upgradeButton.Clicked -= ClickFire;

        BallTreeNodeUI nodeUI = _tree.Get(BallType.Fire);
        nodeUI.OnClick += OpenFire;

        _upgradeButton.HighliterUI.Hide();
        _clickHand.PlayClick(_points[2], _backPanel);
    }

    public void OpenFire(BallTreeNodeUI nodeUI)
    {
        nodeUI.OnClick -= OpenFire;
        nodeUI.HighliterUI.Hide();


        _clickHand.PlayClick(_points[3], _frontPanel);
        _upgradeButton.HighliterUI.Show(3);
        _upgradeButton.Clicked += Close;
    }

    public void Close()
    {
        _upgradeButton.Clicked -= Close;
        _upgradeButton.HighliterUI.Hide();
        _clickHand.PlayClick(_points[4]);
        _exit.Show(3);

        _saveService.GameProgress.BallTreeTutotialComplite = true;
        _saveService.Save();
    }

    public void CloseTutorial()
    {
        _tutorialPanel.Disable();

        foreach (var node in _tree.Nodes)
        {
            node.HighliterUI.Show(3);
        }

        _upgradeButton.HighliterUI.Show(3);
    }
}
