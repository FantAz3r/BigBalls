using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Math = Utils.Math;

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
                SpawnRow();
                _rowIndex++;
            }
        }

        private void SpawnRow()
        {
            var chosenCombination = GenerateRandomValidCombination( _rowWeight, _occupiedPositions);

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

            while (uncheckedIndices.Count > 0)
            {
                int randomIndexPos = Random.Range(0, uncheckedIndices.Count-1);
                int comboIndex = uncheckedIndices[randomIndexPos];
                var enemyCombo = enemyCombinations[comboIndex];
                int enemiesCount = enemyCombo.Count;
                bool isAllEnemiesSame = enemyCombo.All(e => e == enemyCombo[0]);

                IEnumerable<int[]> positionSets;

                if (isAllEnemiesSame)
                    positionSets = Math.GetPositionCombinations(_fieldWidth, enemiesCount);
                else
                    positionSets = Math.GetPositionPermutations(_fieldWidth, enemiesCount);

                var validPlacements = new List<List<(EnemyConfig enemy, Vector2Int position)>>();

                foreach (var positions in positionSets)
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
                        validPlacements.Add(placedEnemies);
                }

                if (validPlacements.Count > 0)
                {
                    return validPlacements[Random.Range(0, validPlacements.Count-1)];
                }
                else
                {
                    uncheckedIndices.RemoveAt(randomIndexPos);
                }
            }
            return null;
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