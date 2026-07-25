using UnityEngine;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TileType
    {
        public GameObject prefab;
        [Range(0, 1)] public float spawnChance; // Вероятность появления этого типа (от 0 до 1)
        public Color debugColor; // Для визуализации в редакторе
    }

    [Header("Settings")]
    [SerializeField] private int _gridSize = 13;
    [SerializeField] private float _tileSize = 1f;
    [SerializeField] private float _noiseScale = 0.3f; // Чем меньше, тем крупнее "острова"

    [Header("Lines Control")]
    [SerializeField] private bool[] _activeLines; // Массив для включения/отключения линий (по индексу 0-12)

    [Header("Prefabs")]
    [SerializeField] private TileType _floorLight;
    [SerializeField] private TileType _floorDark;
    [SerializeField] private List<TileType> _specialGroups; // Вода, ловушки и т.д.

    private void Start ()
    {
        InitializeLinesArray();
        GenerateTile();
    }

    private void InitializeLinesArray ()
    {
        // Если массив не инициализирован или его размер не совпадает с _gridSize
        if (_activeLines == null || _activeLines.Length != _gridSize)
        {
            _activeLines = new bool[_gridSize];
            // По умолчанию все линии включены
            for (int i = 0; i < _gridSize; i++)
            {
                _activeLines[i] = true;
            }
        }
    }

    public void GenerateTile ()
    {
        // Очистка старого тайла, если он был
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        float seed = Random.value * 100f;

        for (int x = 0; x < _gridSize; x++)
        {
            // Проверяем, включена ли текущая линия (по оси X)
            bool isLineActive = x < _activeLines.Length && _activeLines[x];

            // Создаем контейнер для линии (даже если отключена - для структуры, но без блоков)
            GameObject lineContainer = new GameObject($"Line {x}");
            lineContainer.transform.parent = transform;

            // Если линия отключена - просто создаем пустой контейнер и идем дальше
            if (!isLineActive)
                continue;

            for (int z = 0; z < _gridSize; z++)
            {
                GameObject prefabToSpawn = DeterminePrefab(x, z, seed);

                if (prefabToSpawn != null)
                {
                    Vector3 position = new Vector3(x * _tileSize, 0, z * _tileSize);
                    GameObject block = Instantiate(prefabToSpawn, position, Quaternion.identity, lineContainer.transform);
                    block.name = $"Block_{x}_{z}";
                }
            }
        }
    }

    private GameObject DeterminePrefab (int x, int z, float seed)
    {
        float noiseValue = Mathf.PerlinNoise((x + seed) * _noiseScale, (z + seed) * _noiseScale);

        // Суммируем все шансы для нормализации
        float totalChance = 0f;
        foreach (var group in _specialGroups)
            totalChance += group.spawnChance;

        // Если сумма > 1, нормализуем
        if (totalChance > 1f)
        {
            foreach (var group in _specialGroups)
                group.spawnChance /= totalChance;
            totalChance = 1f;
        }

        // Проверяем по диапазонам
        float currentThreshold = 0f;
        foreach (var group in _specialGroups)
        {
            // Проверяем, попадает ли noiseValue в диапазон [currentThreshold, currentThreshold + spawnChance)
            if (noiseValue >= currentThreshold && noiseValue < currentThreshold + group.spawnChance)
            {
                return group.prefab;
            }
            currentThreshold += group.spawnChance;
        }

        // Обычный пол
        bool isLight = (x + z) % 2 == 0;
        return isLight ? _floorLight.prefab : _floorDark.prefab;
    }
}