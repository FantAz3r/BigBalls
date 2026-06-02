using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Math = Utils.Math;
using Random = UnityEngine.Random;

namespace BigBalls.GameplayObjects
{
    public class EnemySpawner
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IEnemyFactory _enemyFactory;
        private readonly TileFactory _tileFactory;

        private Coroutine _spawnCoroutine;
        private int _fieldWidth = 7;
        private int _rowWeight = 4;
        private int _rowIndex = 0;

        private int _debugCounter;

        private WaitForSeconds _fiveSrconds = new WaitForSeconds(5);
        private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();
        private HashSet<Vector2Int> _occupiedPositions = new HashSet<Vector2Int>();

        public EnemySpawner(ICoroutineRunner coroutineRunner, IEnemyFactory enemyFactory, IResourceLoader resourceLoader, TileFactory tileFactory)
        {
            _coroutineRunner = coroutineRunner;
            _enemyFactory = enemyFactory;
            _tileFactory = tileFactory;
            _enemyConfigs = resourceLoader.Load<EnemyData>().Configs;
        }

        public void Start()
        {
            _spawnCoroutine = _coroutineRunner.StartCoroutine(SpawnRoutine());
            _tileFactory.FieldScaled += SetFieldWidth;
        }

        public void Dispose()
        {
            _coroutineRunner.StopCoroutine(_spawnCoroutine);
            _tileFactory.FieldScaled -= SetFieldWidth;
        }

        private void SetFieldWidth(int fieldWidth)
        {
            if (fieldWidth <= _fieldWidth)
                throw new InvalidOperationException();

            _fieldWidth = fieldWidth;
            _rowWeight = (_fieldWidth + 1) / 2;
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)  // добавить игровой стейт, который говорит, когда завершится уровень
            {
                yield return _fiveSrconds;
                _debugCounter = 0;
                SpawnRow();
                Debug.Log(_debugCounter);
                _rowIndex++;
            }
        }

        private void SpawnRow()
        {
            var chosenCombination = GenerateRandomValidCombination(_rowWeight, _occupiedPositions);

            foreach (var spawn in chosenCombination)
            {
                SpawnEnemyAt(spawn.enemy, spawn.position);

                foreach (var block in spawn.enemy.BlocksPositions)
                {
                    var pos = spawn.position + block;
                    _occupiedPositions.Add(pos);
                }
            }
        }

        private List<(EnemyConfig enemy, Vector2Int position)> GenerateRandomValidCombination(int targetWeight, HashSet<Vector2Int> occupiedCellsInPreviousRows)
        {
            List<List<EnemyConfig>> enemyCombinations = new List<List<EnemyConfig>>();
            GenerateEnemyCombinations(targetWeight, null, new List<EnemyConfig>(), enemyCombinations);

            if (enemyCombinations.Count == 0) 
                return null;

            var uncheckedIndices = new List<int>(Enumerable.Range(0, enemyCombinations.Count));
            int totalCombosAttempted = 0;

            while (uncheckedIndices.Count > 0)
            {
                totalCombosAttempted++;
                int randomIndexPos = Random.Range(0, uncheckedIndices.Count-1);
                var enemyCombo = enemyCombinations[uncheckedIndices[randomIndexPos]];

                Dictionary<EnemyConfig, int> enemyGroups = enemyCombo
                    .GroupBy(e => e)
                    .ToDictionary(g => g.Key, g => g.Count());

                var validPlacements = new List<List<(EnemyConfig enemy, Vector2Int position)>>();

                var groupsList = enemyGroups.ToList();
                GenerateGroupPlacements(0, groupsList, new List<(EnemyConfig enemy, Vector2Int position)>(),
                                       new HashSet<Vector2Int>(occupiedCellsInPreviousRows),
                                       _fieldWidth, _rowIndex, validPlacements);

                if (validPlacements.Count > 0)
                    return validPlacements[Random.Range(0, validPlacements.Count-1)];
                else
                    uncheckedIndices.RemoveAt(randomIndexPos);
            }

            return null;
        }

        private void GenerateGroupPlacements(
             int groupIndex,
             List<KeyValuePair<EnemyConfig, int>> groups,
             List<(EnemyConfig enemy, Vector2Int position)> currentPlacement,
             HashSet<Vector2Int> occupiedCells,
             int fieldWidth,
             int rowIndex,
             List<List<(EnemyConfig enemy, Vector2Int position)>> results)
        {
            if (groupIndex >= groups.Count)
            {
                results.Add(new List<(EnemyConfig enemy, Vector2Int position)>(currentPlacement));
                return;
            }

            var currentGroup = groups[groupIndex];
            EnemyConfig enemy = currentGroup.Key;
            int countToPlace = currentGroup.Value;

            var availableX = new List<int>();
            for (int x = 0; x <= fieldWidth-1 - enemy.GetWidth(); x++)
            {
                Vector2Int pos = new Vector2Int(x, rowIndex);
                var enemyCells = enemy.BlocksPositions.Select(b => b + pos);

                if (enemyCells.Any(cell => occupiedCells.Contains(cell)) == false)
                {
                    availableX.Add(x);
                }
            }

            if (availableX.Count < countToPlace)
                return;

            foreach (var xCombo in Math.GetCombinations(availableX, countToPlace))
            {
                var newOccupied = new HashSet<Vector2Int>(occupiedCells);
                var newPlacement = new List<(EnemyConfig enemy, Vector2Int position)>(currentPlacement);
                bool possible = true;

                foreach (int x in xCombo)
                {
                    Vector2Int pos = new Vector2Int(x, rowIndex);
                    var enemyCells = enemy.BlocksPositions.Select(b => b + pos);

                    if (enemyCells.Any(cell => newOccupied.Contains(cell)))
                    {
                        possible = false;
                        break;
                    }

                    foreach (var cell in enemyCells) newOccupied.Add(cell);
                    newPlacement.Add((enemy, pos));
                }

                if (possible)
                {
                    GenerateGroupPlacements(groupIndex + 1, groups, newPlacement, newOccupied, fieldWidth, rowIndex, results);
                }
            }
        }

        private void GenerateEnemyCombinations(int targetWeight, EnemyConfig lastEnemy, List<EnemyConfig> currentCombo, List<List<EnemyConfig>> combinations)
        {
            if (targetWeight == 0)
            {
                combinations.Add(new List<EnemyConfig>(currentCombo));
                return;
            }

            foreach (var enemy in _enemyConfigs)
            {
                if (enemy.Weight > targetWeight) continue;
                if (lastEnemy != null && enemy.Weight > lastEnemy.Weight) continue;

                currentCombo.Add(enemy);
                GenerateEnemyCombinations(targetWeight - enemy.Weight, enemy, currentCombo, combinations);
                currentCombo.RemoveAt(currentCombo.Count - 1);
            }
        }

        private void SpawnEnemyAt(EnemyConfig enemy, Vector2Int position)
        {
            float spawnOffsetX = Mathf.CeilToInt((_fieldWidth - 1) / 2);
            float spawnOffsetY = 0.5f;
            float spawnOffsetZ = 8;

            Vector3 spawnPosition = new Vector3(position.x - spawnOffsetX, spawnOffsetY, spawnOffsetZ);
            _enemyFactory.Create(enemy, spawnPosition);
        }
    }
}