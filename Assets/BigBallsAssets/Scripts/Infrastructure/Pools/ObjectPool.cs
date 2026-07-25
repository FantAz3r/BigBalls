using System.Collections.Generic;
using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer.Unity;

public class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : MonoBehaviour
{
    private T _prefab;
    private IObjectResolverProvider _resolverProvider;
    private int _poolSize;
    private string _nameParent;
    private GameObject _positionInHierarchy;

    private Queue<T> _objectPool = new Queue<T>();

    public ObjectPool (int initializePoolSize, IObjectResolverProvider resolverProvider)
    {
        _poolSize = initializePoolSize;
        _resolverProvider = resolverProvider;
    }

    public T Get ()
    {
        return Get(Vector3.zero, Quaternion.identity, _positionInHierarchy.transform);
    }

    public T Get (Vector3 position)
    {
        return Get(position, Quaternion.identity, _positionInHierarchy.transform);
    }

    public T Get (Vector3 position, Quaternion rotation)
    {
        return Get(position, rotation, _positionInHierarchy.transform);
    }

    public T Get (Transform parent)
    {
        return Get(Vector3.zero, Quaternion.identity, parent);
    }

    public T Get (Vector3 position, Quaternion rotation, Transform parent)
    {
        if (_objectPool.Count == 0)
            ExpandPool(position, rotation, parent);

        T obj = _objectPool.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        if (parent != null)
            obj.transform.SetParent(parent);

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release (T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_positionInHierarchy.transform);
        _objectPool.Enqueue(obj);
    }
   
    public override void Clear ()
    {
        foreach (T obj in _objectPool)
        {
            if (obj != null)
                GameObject.Destroy(obj.gameObject);
        }

        _objectPool.Clear();
    }

    public void InitializePool (T prefab, Transform transformParent, string nameParent)
    {
        _prefab = prefab;
        _nameParent = nameParent;
        _positionInHierarchy = new GameObject(_nameParent);
        _positionInHierarchy.transform.SetParent(transformParent);

        for (int i = 0; i < _poolSize; i++)
        {
            ExpandPool(Vector3.zero, Quaternion.identity, _positionInHierarchy.transform);
        }
    }

    private T CreateObject (Vector3 position, Quaternion rotation, Transform parent)
    {
        T obj = _resolverProvider.CurrentResolver.Instantiate(_prefab, position, rotation);
        _resolverProvider.CurrentResolver.Inject(obj);
        obj.name = _nameParent;

        if (parent != null)
            obj.transform.SetParent(parent);
        else
            obj.transform.SetParent(_positionInHierarchy.transform);

        obj.gameObject.SetActive(false);
        return obj;
    }

    private void ExpandPool (Vector3 position, Quaternion rotation, Transform parent)
    {
        T obj = CreateObject(position, rotation, parent);
        _objectPool.Enqueue(obj);
    }
}