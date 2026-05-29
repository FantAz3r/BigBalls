using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBalls.GameplayObjects
{
    public class EnemySpawner
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IEnemyFactory _enemyFactory;

        private WaitForSeconds _fiveSrconds = new WaitForSeconds(0.5f);
        private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();
        private int _fieldWidth = 7;
        private int _rowWeight = 4;
        private int _rowIndex = 0;
        private HashSet<Vector2Int> _occupiedPositions = new HashSet<Vector2Int>();

        public EnemySpawner(ICoroutineRunner coroutineRunner, IEnemyFactory enemyFactory, IResourceLoader resourceLoader)
        {
            _coroutineRunner = coroutineRunner;
            _enemyFactory = enemyFactory;
            _enemyConfigs = resourceLoader.Load<EnemyData>().Configs;
        }

        public void Start()
        {
            _coroutineRunner.StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)  // добавить игровой стейт, который говорит, когда завершится уровень

            {
                yield return _fiveSrconds;
                SpawnRow();
                _rowIndex++;
            }
        }

        private void SpawnRow()
        {
            var allCombinations = GenerateAllCombinations( _rowWeight, _occupiedPositions);

            if (allCombinations.Count == 0)
                throw new InvalidOperationException(nameof(allCombinations));

            var chosenCombination = allCombinations[Random.Range(0, allCombinations.Count - 1)];

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

        private List<List<(EnemyConfig enemy, Vector2Int position)>> GenerateAllCombinations( int targetWeight, HashSet<Vector2Int> occupiedCellsInPreviousRows)
        {
            var results = new List<List<(EnemyConfig enemy, Vector2Int position)>>();

            List<List<EnemyConfig>> enemyCombinations = new List<List<EnemyConfig>>();
            GenerateEnemyCombinations(targetWeight, null, new List<EnemyConfig>(), enemyCombinations);

            foreach (var enemyCombo in enemyCombinations)
            {

                int enemiesCount = enemyCombo.Count;

                var positionPermutations = GetPositionPermutations( enemiesCount);

                foreach (var positions in positionPermutations)
                {
                    bool validPlacement = true;
                    var currentOccupied = new HashSet<Vector2Int>(occupiedCellsInPreviousRows);
                    var placedEnemies = new List<(EnemyConfig enemy, Vector2Int position)>();

                    for (int i = 0; i < enemiesCount; i++)
                    {
                        EnemyConfig enemy = enemyCombo[i];

                        int xPos = positions[i];
                        Vector2Int pos = new Vector2Int(xPos, _rowIndex);

                        if (xPos + enemy.GetWidth() > _fieldWidth-1)
                        {
                            validPlacement = false;
                            break;
                        }

                        var enemyCells = enemy.BlocksPositions.Select(b => b + pos);

                        if (enemyCells.Any(cell => currentOccupied.Contains(cell)))
                        {
                            validPlacement = false;
                            break;
                        }

                        foreach (var cell in enemyCells)
                            currentOccupied.Add(cell);

                        placedEnemies.Add((enemy, pos));
                    }

                    if (validPlacement)
                        results.Add(placedEnemies);
                }
            }

            return results;
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

        private List<int[]> GetPositionPermutations(int enemiesCount)
        {
            var results = new List<int[]>();

            void Backtrack(List<int> path, HashSet<int> used)
            {
                if (path.Count == enemiesCount)
                {
                    results.Add(path.ToArray());
                    return;
                }
                for (int i = 0; i < _fieldWidth; i++)
                {
                    if (used.Contains(i)) continue;
                    path.Add(i);
                    used.Add(i);
                    Backtrack(path, used);
                    used.Remove(i);
                    path.RemoveAt(path.Count - 1);
                }
            }

            Backtrack(new List<int>(), new HashSet<int>());
            return results;
        }

        private void SpawnEnemyAt(EnemyConfig enemy, Vector2Int position)
        {
            float spawnOffsetX = Mathf.CeilToInt((_fieldWidth - 1) / 2);
            float spawnOffsetY = 0.5f; 
            Vector3 spawnPosition = new Vector3(position.x, spawnOffsetY, position.y);
            _enemyFactory.Create(enemy, spawnPosition);
        }
    }
}