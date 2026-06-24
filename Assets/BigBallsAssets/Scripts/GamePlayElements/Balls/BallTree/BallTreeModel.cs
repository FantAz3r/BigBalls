using System.Collections.Generic;
using System.Linq;
using BigBalls.Configs;
using BigBalls.Services;
using BigBalls.StaticData;
using UnityEngine;

public class BallTreeModel
{
    private readonly BallsRepository _ballsRepository;

    private Dictionary<BallType, BallConfig> _nodes;
    private Dictionary<BallType, List<BallType>> _children;
    private Dictionary<BallType, List<BallType>> _parents;
    private BallType _rootType;

    public BallTreeModel (IResourceLoader resourceLoader, BallsRepository ballsRepository)
    {
        BallTreeData treeData = resourceLoader.Load<BallTreeData>();
        _ballsRepository = ballsRepository;
        _nodes = treeData.Nodes.ToDictionary(n => n.BallType);
        _rootType = treeData.RootBallType;

        BuildHierarchy();
        CalculatePositions(treeData);
    }

    public void CreateTree()
    {

    }

    private void BuildHierarchy ()
    {
        _children = new Dictionary<BallType, List<BallType>>();
        _parents = new Dictionary<BallType, List<BallType>>();

        foreach (var node in _nodes.Values)
        {
            if (_children.ContainsKey(node.BallType) == false)
                _children[node.BallType] = new List<BallType>();

            if (_parents.ContainsKey(node.BallType) == false)
                _parents[node.BallType] = new List<BallType>();

            foreach (var parentType in node.ParentTypes ?? new List<BallType>())
            {
                if (_children.ContainsKey(parentType) == false)
                    _children[parentType] = new List<BallType>();

                _children[parentType].Add(node.BallType);
                _parents[node.BallType].Add(parentType);
            }
        }
    }

    private void CalculatePositions (BallTreeData treeData)
    {
        if (_nodes.ContainsKey(_rootType) == false)
            return;

        var rootPos = Vector2.zero;
        _nodes[_rootType].Position = rootPos;

        var queue = new Queue<(BallType type, int depth, int index, Vector2 parentPos)>();
        queue.Enqueue((_rootType, 0, 0, rootPos));

        while (queue.Count > 0)
        {
            var (type, depth, index, parentPos) = queue.Dequeue();
            var node = _nodes[type];

            if (_children.TryGetValue(type, out var children) == false)
                continue;

            var childCount = children.Count;

            for (int i = 0; i < childCount; i++)
            {
                var childType = children[i];

                if (_nodes.ContainsKey(childType) == false)
                    continue;

                var childNode = _nodes[childType];

                if (childNode.Position == Vector2.zero)
                {
                    var xOffset = (i - (childCount - 1) / 2f) * treeData.Spacing.x;
                    var yOffset = -treeData.Spacing.y;
                    childNode.Position = new Vector2(
                        parentPos.x + xOffset,
                        parentPos.y + yOffset
                    );
                }

                queue.Enqueue((childType, depth + 1, i, childNode.Position));
            }
        }
    }

    public BallConfig GetNode (BallType type) => _nodes.GetValueOrDefault(type);
    public List<BallType> GetChildren (BallType type) => _children.GetValueOrDefault(type) ?? new List<BallType>();
    public List<BallType> GetParents (BallType type) => _parents.GetValueOrDefault(type) ?? new List<BallType>();
    public bool IsRoot (BallType type) => type == _rootType;
    public IEnumerable<BallType> GetAllTypes () => _nodes.Keys;

    public bool CanUnlock (BallType type)
    {
        var node = GetNode(type);

        if (node == null)
            return false;

        if (_ballsRepository.AllModels.TryGetValue(type, out var ball) && ball.IsOpen)
            return false;

        foreach (var parentType in node.ParentTypes ?? new List<BallType>())
        {
            if (_ballsRepository.AllModels.TryGetValue(parentType, out var parentBall) == false)
                return false;

            if (parentBall.IsOpen == false)
                return false;

            if (parentBall.ItemEXP < node.RequiredEXP)
                return false;
        }

        return true;
    }
}