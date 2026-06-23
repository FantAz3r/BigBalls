using System.Collections.Generic;
using System.Linq;
using BigBalls.StaticData;
using BigBalls.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BallTreeUI : WindowBase
{
    [SerializeField] private RectTransform _treeContainer;
    [SerializeField] private BallTreeNodeUI _nodePrefab;
    [SerializeField] private BallTreeConnectionUI _connectionPrefab;
    [SerializeField] private ScrollRect _scrollRect;

    private BallTreeController _controller;
    private Dictionary<BallType, BallTreeNodeUI> _nodeUIs = new();
    private List<BallTreeConnectionUI> _connections = new();

    [Inject]
    public void Construct (BallTreeController controller)
    {
        _controller = controller;
        BuildTreeUI();
        UpdateUI();
    }

    private void BuildTreeUI ()
    {
        // Очищаем старые элементы
        foreach (Transform child in _treeContainer)
            Destroy(child.gameObject);

        _nodeUIs.Clear();
        _connections.Clear();

        var model = _controller.GetTreeModel();
        var allTypes = model.GetAllTypes().ToList();

        // Создаем UI для каждого узла
        foreach (var type in allTypes)
        {
            var node = model.GetNode(type);
            var uiElement = Instantiate(_nodePrefab, _treeContainer);
            uiElement.Initialize(type, node, _controller);
            uiElement.OnClick += OnNodeClicked;
            _nodeUIs[type] = uiElement;
            uiElement.gameObject.SetActive(true);
            // Устанавливаем позицию
            uiElement.RectTransform.anchoredPosition = node.Position;
        }

        // Создаем соединения между узлами
        foreach (var type in allTypes)
        {
            var children = model.GetChildren(type);
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

        UpdateContainerSize();
    }

    private void UpdateContainerSize ()
    {
        if (_nodeUIs.Count == 0) return;

        var minPos = Vector2.zero;
        var maxPos = Vector2.zero;

        foreach (var ui in _nodeUIs.Values)
        {
            var pos = ui.RectTransform.anchoredPosition;
            minPos = Vector2.Min(minPos, pos);
            maxPos = Vector2.Max(maxPos, pos);
        }

        var size = maxPos - minPos + new Vector2(200f, 200f);
        _treeContainer.sizeDelta = size;

        // Центрируем содержимое
        var center = (minPos + maxPos) / 2f;
        foreach (var ui in _nodeUIs.Values)
        {
            ui.RectTransform.anchoredPosition -= center;
        }

        // Обновляем соединения
        foreach (var connection in _connections)
        {
            connection.UpdatePositions();
        }
    }

    private void OnNodeClicked (BallType type)
    {
        // Показываем информацию о шаре
        Debug.Log($"Clicked on ball: {type}");

        if (_controller.CanUnlockBall(type))
        {
            // Показываем кнопку "Купить"
            _controller.TryUnlockBall(type);
        }
    }

    public void UpdateUI ()
    {
        foreach (var ui in _nodeUIs.Values)
        {
            if (ui != null && ui.gameObject != null)
            {
                // Убеждаемся, что нода активна
                if (ui.gameObject.activeSelf == false)
                    ui.gameObject.SetActive(true);

                ui.UpdateState();
            }
        }
    }
}
