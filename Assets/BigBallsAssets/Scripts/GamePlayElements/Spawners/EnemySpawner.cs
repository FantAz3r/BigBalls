using BigBalls.Factories;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Math = Utils.Math;
using Random = UnityEngine.Random;

namespace BigBalls.GameplayObjects
{
    public class EnemySpawner
    {
        private readonly IEnemyFactory _enemyFactory;
        private readonly IWinService _winService;

        private int _fieldWidth = 7;
        private int _rowWeight = 4;
        private int _rowIndex = 0;

        private HashSet<Vector2Int> _occupiedPositions = new HashSet<Vector2Int>();
        private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();

        public EnemySpawner(IEnemyFactory enemyFactory, IWinService winService)
        {
            _enemyFactory = enemyFactory;
            _winService = winService;
        }

        public void Init(List<EnemyConfig> enemies)
        {
            _enemyConfigs = enemies;
        }

        public void Reset()
        {
            _rowIndex = 0;
            _occupiedPositions.Clear();
        }

        public void ScaleField(int fieldWidth)
        {
            if (fieldWidth <= _fieldWidth)
                throw new InvalidOperationException();

            _fieldWidth = fieldWidth;
            _rowWeight = (_fieldWidth + 1) / 2;
        }

        public Boss SpawnBoss(EnemyConfig enemyConfig)
        {
            int centerX = (_fieldWidth - 1) / 2;
            Vector2Int spawnPosition = new Vector2Int(centerX, 0);
            MarkOccupied(spawnPosition, enemyConfig);

            return SpawnEnemyAt(enemyConfig, spawnPosition) as Boss;
        }

        public void SpawnLevelBoss(EnemyConfig enemyConfig)
        {
            _winService.SetWinReason(SpawnBoss(enemyConfig) as LevelBoss);
        }

        public void SpawnLine()
        {
            var chosenCombination = GenerateRandomValidCombination(_rowWeight, _occupiedPositions);

            foreach (var spawn in chosenCombination)
            {
                SpawnEnemyAt(spawn.enemy, spawn.position);
                MarkOccupied(spawn.position, spawn.enemy);
            }

            _rowIndex++;
        }

        private void MarkOccupied(Vector2Int position, EnemyConfig config)
        {
            foreach (var blockPosition in config.BlocksPositions)
            {
                _occupiedPositions.Add(position + blockPosition);
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
                int randomIndexPos = Random.Range(0, uncheckedIndices.Count - 1);
                var enemyCombo = enemyCombinations[uncheckedIndices[randomIndexPos]];

                Dictionary<EnemyConfig, int> enemyGroups = enemyCombo
                    .GroupBy(enemy => enemy)
                    .ToDictionary(group => group.Key, group => group.Count());

                var validPlacements = new List<List<(EnemyConfig enemy, Vector2Int position)>>();

                var groupsList = enemyGroups.ToList();
                GenerateGroupPlacements(0, groupsList, new List<(EnemyConfig enemy, Vector2Int position)>(),
                                       new HashSet<Vector2Int>(occupiedCellsInPreviousRows),
                                       _fieldWidth, _rowIndex, validPlacements);

                if (validPlacements.Count > 0)
                    return validPlacements[Random.Range(0, validPlacements.Count - 1)];
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

            for (int x = 0; x <= fieldWidth - 1 - enemy.GetWidth(); x++)
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

                    foreach (var cell in enemyCells)
                        newOccupied.Add(cell);

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

        private Enemy SpawnEnemyAt(EnemyConfig enemy, Vector2Int position)
        {
            float spawnOffsetX = Mathf.CeilToInt((_fieldWidth - 1) / 2);
            float spawnOffsetY = 0.5f;
            float spawnOffsetZ = 8;

            Vector3 spawnPosition = new Vector3(position.x - spawnOffsetX, spawnOffsetY, spawnOffsetZ);
            return _enemyFactory.Create(enemy, spawnPosition);
        }
    }
}