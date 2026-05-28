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

        private WaitForSeconds _fiveSrconds = new WaitForSeconds(1);
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
            while (true)  // добавить игровой стейт, который говорит, когда завершитс€ уровень

            {
                yield return _fiveSrconds;

                SpawnRow(_rowIndex++);
            }
        }

        private void SpawnRow(int rowIndex)
        {
            // √енерируем все возможные комбинации врагов по весу 4 на ширине линии с учЄтом зан€тостей
            var allCombinations = GenerateValidCombinations(rowIndex, 0, _rowWeight);

            if (allCombinations.Count == 0)
                throw new InvalidOperationException(nameof(allCombinations));

            var chosenCombination = allCombinations[Random.Range(0, allCombinations.Count - 1)];

            foreach (var spawn in chosenCombination)
            {
                SpawnEnemyAt(spawn.enemy, spawn.position + new Vector2Int(0, rowIndex));

                // ѕомечаем зан€тость блоков врага
                foreach (var block in spawn.enemy.BlocksPositions)
                {
                    var pos = spawn.position + block + new Vector2Int(0, rowIndex);
                    _occupiedPositions.Add(pos);
                }
            }
        }

        private List<List<(EnemyConfig enemy, Vector2Int position)>> GenerateValidCombinations(int rowIndex, int currentX, int remainingWeight)
        {
            var results = new List<List<(EnemyConfig enemy, Vector2Int position)>>();

            // Ѕазовый случай: если вес достигнут или вышли за пределы ширины
            if (remainingWeight == 0 || currentX >= _fieldWidth)
            {
                if (remainingWeight == 0)
                    results.Add(new List<(EnemyConfig enemy, Vector2Int position)>());
                return results;
            }

            bool anyPlacement = false;

            foreach (var enemy in _enemyConfigs)
            {
                if (enemy.Weight > remainingWeight)
                    continue;

                if (CanPlaceEnemy(enemy, new Vector2Int(currentX, rowIndex)))
                {
                    anyPlacement = true;

                    var blocksInRow = enemy.BlocksPositions.Where(block => block.y == 0).ToList();
                    if (blocksInRow.Count == 0)
                        blocksInRow = new List<Vector2Int>() { new Vector2Int(0, 0) };

                    int maxBlockXInRow = blocksInRow.Max(block => block.x);
                    int nextX = currentX + maxBlockXInRow + 1;

                    var nextCombinations = GenerateValidCombinations(rowIndex, nextX, remainingWeight - enemy.Weight);

                    foreach (var comb in nextCombinations)
                    {
                        var newList = new List<(EnemyConfig enemy, Vector2Int position)>() { (enemy, new Vector2Int(currentX, 0)) };
                        newList.AddRange(comb);
                        results.Add(newList);
                    }
                }
            }

            // ≈сли не удалось поставить ни одного врага Ч пропускаем клетку и идЄм дальше,
            // т.к. нельз€ ставить врага здесь (или нет подход€щих по весу)
            if (anyPlacement == false)
            {
                var skipCombs = GenerateValidCombinations(rowIndex, currentX + 1, remainingWeight);
                foreach (var comb in skipCombs)
                {
                    results.Add(new List<(EnemyConfig enemy, Vector2Int position)>(comb));
                }
            }

            Debug.Log(results.Count);
            return results;
        }

        private bool CanPlaceEnemy(EnemyConfig enemy, Vector2Int headPosition)
        {
            foreach (var blockOffset in enemy.BlocksPositions)
            {
                var pos = headPosition + blockOffset;

                if (_occupiedPositions.Contains(pos))
                    return false;

                if (pos.x < 0 || pos.x >= _fieldWidth)
                    return false; 
            }
            return true;
        }

        private void SpawnEnemyAt(EnemyConfig enemy, Vector2Int position)
        {
            float spawnOffset = Mathf.CeilToInt((_fieldWidth - 1) / 2);
            Vector3 spawnPosition = new Vector3(position.x - spawnOffset, 0f, position.y);
            _enemyFactory.Create(enemy, spawnPosition);
        }
    }
}