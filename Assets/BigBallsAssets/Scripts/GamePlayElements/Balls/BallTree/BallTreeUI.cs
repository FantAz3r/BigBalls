using System.Collections.Generic;
using System.Linq;
using BigBalls.StaticData;
using BigBalls.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BallTreeUI : WindowBase, IResetble
{
    [SerializeField] private RectTransform _treeContainer;
    [SerializeField] private BallTreeNodeUI _nodePrefab;
    [SerializeField] private BallTreeConnectionUI _connectionPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    private IBallUnlockService _unlockService;
    private BallTreeModel _treeModel;
    private Dictionary<BallType, BallTreeNodeUI> _nodeUIs = new();
    private List<BallTreeConnectionUI> _connections = new();

    private void OnDestroy ()
    {
        foreach (var node in _nodeUIs.Values)
        {
            node.OnClick -= OnNodeClicked;
        }
    }

    [Inject]
    public void Construct (BallTreeModel treeModel, ISaveService saveService, IBallUnlockService unlockService)
    {
        _treeModel = treeModel;
        _unlockService = unlockService;

        saveService.RegisterResetable(this);
        BuildTreeUI();
        UpdateUI();
    }

    public void UpdateUI ()
    {
        foreach (var ui in _nodeUIs.Values)
        {
            if (ui != null && ui.gameObject != null)
            {
                ui.UpdateState(_treeModel.GetNode(ui.BallType));
            }
        }

        foreach (var line in _connections)
        {
            line.UpdateLine();
        }
    }

    public void Reset () => UpdateUI();

    private void BuildTreeUI ()
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
            var uiElement = Instantiate(_nodePrefab, _treeContainer);
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
                if (_nodeUIs.ContainsKey(type) && _nodeUIs.ContainsKey(childType))
                {
                    var connection = Instantiate(_connectionPrefab, _treeContainer);
                    connection.SetConnection(
                        _nodeUIs[type].RectTransform,
                        _nodeUIs[childType].RectTransform
                    );

                    _connections.Add(connection);
                }
            }
        }
    }

    private void UpdateContainerSize ()
    {
        if (_nodeUIs.Count == 0) return;

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

    private void OnNodeClicked (BallType type)
    {
        _unlockService.TryUnlockBall(type);
        UpdateUI();
    }
}
