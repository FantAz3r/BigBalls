using UnityEngine;

public interface IObjectPool<T> where T : Component
{
    void InitializePool(T configPrefab, Transform enemyPoolTransform, string nameParent);
}