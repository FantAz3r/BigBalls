using System.Collections.Generic;
using BigBalls.StaticData;
using UnityEngine;
using VContainer;

public class BallTreeTutorial : MonoBehaviour
{
    [SerializeField] private List<Vector3> _points = new List<Vector3>();

    [SerializeField] private HighliterUI _statsPanel;
    [SerializeField] private TutorialPanel _tutorialPanel;
    [SerializeField] private BallTreeUI _tree;
    [SerializeField] private ClickHand _clickHand;
    [SerializeField] private TextViewer _textViewer;
    [SerializeField] private HighliterUI _exit;

    [SerializeField] private RectTransform _frontPanel;
    [SerializeField] private RectTransform _backPanel;

    [SerializeField] private ItemUpgradeButton _upgradeButton;

    private ISaveService _saveService;
    private GlobalWallet _globalWallet;
    private float _addedEXP;
    private float _addedGold;

    private bool _isTutorialComplite;

    [Inject]
    public void Construct (ISaveService saveService, GlobalWallet globalWallet)
    {
        _saveService = saveService;
        _globalWallet = globalWallet;
    }

    private void OnEnable ()
    {
        _addedEXP = 0;
        _addedGold = 0;

        if (_saveService == null)
            return;

        _tree.Inited += WriteStartText;

    }

    private void OnDisable ()
    {
        _tree.Inited -= WriteStartText;

        if (_isTutorialComplite == false)
        {
            _globalWallet.TrySpendCoins((int) _addedGold);
            BallTreeNodeUI nodeUI = _tree.Get(BallType.Fire);
            nodeUI.BallModel.RemoveItemEXP(_addedEXP);
        }
    }

    private void WriteStartText ()
    {
        if (_saveService.GameProgress.BallTreeTutotialComplite)
        {
            CloseTutorial();
            return;
        }

        _textViewer.WriteNext();
        _tutorialPanel.Clicked += ClickBase;
    }


    public void ClickBase ()
    {
        _tutorialPanel.Clicked -= ClickBase;
        _textViewer.WriteNext();



        _clickHand.PlayClick(_points[0], _backPanel);
        BallTreeNodeUI nodeUI = _tree.Get(BallType.Base);
        nodeUI.OnClick += UpgradeBase;
        nodeUI.HighliterUI.Show();
        _clickHand.transform.localScale = Vector3.one;
    }

    public void UpgradeBase (BallTreeNodeUI nodeUI)
    {
        nodeUI.OnClick -= UpgradeBase;
        nodeUI.HighliterUI.Hide();
        _addedEXP = nodeUI.BallModel.EXPForNextLevel;
        nodeUI.BallModel.AddItemEXP(_addedEXP);
        nodeUI.UpdateState();

        _clickHand.PlayClick(_points[1], _frontPanel);
        _upgradeButton.HighliterUI.Show(3);
        _upgradeButton.Clicked += ClickFire;
    }

    public void ClickFire ()
    {
        _textViewer.WriteNext();

        _upgradeButton.Clicked -= ClickFire;

        BallTreeNodeUI nodeUI = _tree.Get(BallType.Fire);
        nodeUI.HighliterUI.Show();
        nodeUI.OnClick += OpenFire;

        _upgradeButton.HighliterUI.Hide();
        _clickHand.PlayClick(_points[2], _backPanel);
    }

    public void OpenFire (BallTreeNodeUI nodeUI)
    {
        nodeUI.OnClick -= OpenFire;
        nodeUI.HighliterUI.Hide();

        _addedGold = nodeUI.BallModel.BallConfig.UnlockPrice;
        _globalWallet.AddCoins(_addedGold);
        _clickHand.PlayClick(_points[3], _frontPanel);
        _upgradeButton.HighliterUI.Show();
        _upgradeButton.Clicked += Close;
    }

    public void Close ()
    {
        _textViewer.WriteNext();

        _upgradeButton.Clicked -= Close;
        _upgradeButton.HighliterUI.Hide();
        _clickHand.PlayClick(_points[4]);
        _exit.Show();

        _saveService.GameProgress.BallTreeTutotialComplite = true;
        _saveService.Save();
    }

    public void CloseTutorial ()
    {
        _tutorialPanel.Disable();
        _clickHand.Disable();

        foreach (var node in _tree.Nodes)
        {
            node.HighliterUI.Show();
        }

        _upgradeButton.HighliterUI.Show(4);
        _exit.Show();
        _statsPanel.Show();
    }
}
