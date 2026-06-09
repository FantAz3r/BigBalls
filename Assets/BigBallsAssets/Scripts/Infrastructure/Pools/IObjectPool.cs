using UnityEngine;

public interface IObjectPool<T> where T : MonoBehaviour
{
    void InitializePool(T configPrefab, Transform enemyPoolTransform, string nameParent);
}