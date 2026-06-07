using System;
using System.Collections.Generic;
using BigBalls.Infrastructure.DI;
using Unity.Mathematics;
using UnityEngine;
using VContainer.Unity;

public class ObjectPool<T> where  T : MonoBehaviour
{
    private T _prefab;
    private IObjectResolverProvider _resolverProvider;
    private int _poolSize;
    private string _nameParent;
    private GameObject _positionInHierarchy;

    private Queue<T> _objectPools = new Queue<T>();
    
    public ObjectPool(int initializePoolSize, IObjectResolverProvider resolverProvider)
    {
        _poolSize = initializePoolSize;
        _resolverProvider = resolverProvider;
    }
    
    public T Get()
    {
        if (_objectPools.Count == 0)
            ExpandPool();

        T obj = _objectPools.Dequeue();
        obj.gameObject.SetActive(true);

        return obj;
    }
    
    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        _objectPools.Enqueue(obj);
    }

    public void InitializePool(T prefab, Transform transformParent, string nameParent)
    {
        _prefab = prefab;
        _nameParent = nameParent;
        _positionInHierarchy = new GameObject(_nameParent);
        _positionInHierarchy.transform.SetParent(transformParent);
        
        for (int i = 0; i < _poolSize; i++)
        {
            ExpandPool();
           
        }
    }
    
    private void ExpandPool()
    {
        T obj = _resolverProvider.CurrentResolver.Instantiate(_prefab, Vector3.zero, quaternion.identity);
        obj.name = _nameParent;
        obj.transform.SetParent(_positionInHierarchy.transform);
        obj.gameObject.SetActive(false);
        _objectPools.Enqueue(obj);
    }
}