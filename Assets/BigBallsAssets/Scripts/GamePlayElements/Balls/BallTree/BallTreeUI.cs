using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.StaticData;
using BigBalls.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class BallTreeUI : WindowBase, IResetble
{
    [SerializeField] private RectTransform _treeContainer;
    [SerializeField] private BallTreeNodeUI _nodePrefab;
    [SerializeField] private BallTreeConnectionUI _connectionPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private UpgradePanel _upgradePanel;

    private IObjectResolverProvider _objectResolverProvider;
    private ISaveService _saveService;
    private IBallUnlockService _unlockService;
    private BallTreeModel _treeModel;
    private Dictionary<BallType, BallTreeNodeUI> _nodeUIs = new();
    private List<BallTreeConnectionUI> _connections = new();

    private void OnDestroy()
    {
        foreach (var node in _nodeUIs.Values)
        {
            node.OnClick -= OnNodeClicked;
        }
    }

    [Inject]
    public void Construct(
        BallTreeModel treeModel,
        ISaveService saveService,
        IBallUnlockService unlockService,
        IResourceLoader resourceLoader,
        IObjectResolverProvider objectResolverProvider)
    {
        _objectResolverProvider = objectResolverProvider;
        _saveService = saveService;
        _treeModel = treeModel;
        _unlockService = unlockService;
        _upgradePanel.Init(resourceLoader.Load<StatTextHolder>());
        saveService.RegisterResetable(this);
        BuildTreeUI();
        UpdateUI();
    }

    public void UpdateUI(BallTreeNodeUI ballTreeNodeUI = null)
    {
        foreach (var ui in _nodeUIs.Values)
        {
            if (ui != null && ui.gameObject != null)
            {
                ui.UpdateState(_treeModel.GetNode(ui.BallType));
            }
        }
    }

    public void Reset()
    {
        BuildTreeUI();
        UpdateUI();
    }

    private void BuildTreeUI()
    {
        if (_treeContainer.childCount > 0)
        {
            foreach (Transform child in _treeContainer)
                Destroy(child.gameObject);

            _nodeUIs.Clear();
            _connections.Clear();
        }

        var allTypes = _treeModel.GetAllTypes().ToList();

        foreach (var type in allTypes)
        {
            var node = _treeModel.GetNode(type);
            var uiElement = _objectResolverProvider.CurrentResolver.Instantiate(_nodePrefab, _treeContainer);

            uiElement.Init(type, node, _unlockService);
            uiElement.OnClick += OnNodeClicked;

            _nodeUIs[type] = uiElement;
            uiElement.gameObject.SetActive(true);
            uiElement.RectTransform.anchoredPosition = node.BallConfig.Position;
        }

        UpdateContainerSize();

        foreach (var type in allTypes)
        {
            var children = _treeModel.GetChildren(type);

            foreach (var childType in children)
            {
                if (_nodeUIs.TryGetValue(type, out BallTreeNodeUI parentUI) &&
                    _nodeUIs.TryGetValue(childType, out var childUI))
                {
                    BallTreeConnectionUI connection = _objectResolverProvider.CurrentResolver.Instantiate(_connectionPrefab, _treeContainer);
                    connection.SetConnection(
                        parentUI.RectTransform,
                        childUI.RectTransform
                    );

                    _connections.Add(connection);
                    parentUI.AddChildConnection(connection);
                    childUI.AddParentConnection(connection);
                }
            }
        }
    }

    private void UpdateContainerSize()
    {
        if (_nodeUIs.Count == 0)
            return;

        var minPos = Vector2.zero;
        var maxPos = Vector2.zero;
        var size = maxPos - minPos + new Vector2(200f, 200f);
        var center = (minPos + maxPos) / 2f;

        foreach (var ui in _nodeUIs.Values)
        {
            var pos = ui.RectTransform.anchoredPosition;
            minPos = Vector2.Min(minPos, pos);
            maxPos = Vector2.Max(maxPos, pos);
        }

        _treeContainer.sizeDelta = size;

        foreach (var ui in _nodeUIs.Values)
        {
            ui.RectTransform.anchoredPosition -= center;
        }
    }

    private void OnNodeClicked(BallTreeNodeUI node)
    {
        _upgradePanel.SetNode(node);
        UpdateUI();
    }
}
