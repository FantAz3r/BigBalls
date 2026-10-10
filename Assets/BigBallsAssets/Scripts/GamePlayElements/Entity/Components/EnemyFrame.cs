using System.Collections.Generic;
using BigBalls.GameplayObjects;
using CustomInspector;
using UnityEngine;

public class EnemyFrame : MonoBehaviour
{
    [SerializeField] private MaterialType _currentMaterialType;
    [SerializeField] private Renderer  _renderer;
    [SerializeField] private SerializableDictionary<MaterialType, Material> _materials = new ();

    private void Awake ()
    {
        ApplyMaterial();
    }

#if UNITY_EDITOR
    private void OnValidate ()
    {
        ApplyMaterial();
    }
#endif

    private void ApplyMaterial ()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        if (_renderer == null || _materials == null)
            return;

        if (!_materials.TryGetValue(_currentMaterialType, out Material material) || material == null)
            return;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            _renderer.sharedMaterial = material;
            return;
        }
#endif

        _renderer.material = material;
    }
}

