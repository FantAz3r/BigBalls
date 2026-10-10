using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class Math
    {
        public static List<int[]> GetPositionCombinations (int n, int k) => GetPositionCombinationsOrPermutations(n, k, false);
        public static List<int[]> GetPositionPermutations (int n, int k) => GetPositionCombinationsOrPermutations(n, k, true);

        private static List<int[]> GetPositionCombinationsOrPermutations (int n, int k, bool usePermutations)
        {
            var results = new List<int[]>();

            void Backtrack (List<int> current, HashSet<int> used, int start)
            {
                if (current.Count == k)
                {
                    results.Add(current.ToArray());
                    return;
                }

                for (int i = usePermutations ? 0 : start; i < n; i++)
                {
                    if (usePermutations && used.Contains(i)) continue;

                    current.Add(i);
                    if (usePermutations) used.Add(i);

                    Backtrack(current, used, usePermutations ? 0 : i + 1);

                    if (usePermutations) used.Remove(i);
                    current.RemoveAt(current.Count - 1);
                }
            }

            Backtrack(new List<int>(), new HashSet<int>(), 0);
            return results;
        }

        public static IEnumerable<IEnumerable<T>> GetCombinations<T> (IEnumerable<T> list, int length)
        {
            if (length == 0)
                return new[] { Enumerable.Empty<T>() };

            return list.SelectMany((e, i) =>
                GetCombinations(list.Skip(i + 1), length - 1).Select(c => (new[] { e }).Concat(c)));
        }

        public static float Additive (float baseValue, float increasePerLevel, int level)
            => baseValue + increasePerLevel * level;

        public static float Multiplicative (float baseValue, float multiplierPerLevel, int level)
        {
            float result = baseValue * Mathf.Pow(multiplierPerLevel, level);
            return (int) Mathf.Floor(result);
        }

        public static float PercentAdditive (float basePercent, float increasePercentPerLevel, int level)
            => basePercent + increasePercentPerLevel * level;

        public static float PercentMultiplicative (float basePercent, float multiplierPerLevel, int level)
        {
            float result = basePercent * Mathf.Pow(multiplierPerLevel, level);
            return Mathf.Min(result, 1);
        }

        public static float PercentDiminishingReturnsLimited (float basePercent, float maxIncrease, float factor, int level)
        {
            float increment = maxIncrease * (1 - 1 / (1 + (float) level / factor));
            float result = basePercent + increment;
            return Mathf.Min(result, 1);
        }

        public static Vector3[] CalculateParabolaPath (Vector3 start, Vector3 end, float height)
        {
            Vector3 midPoint = (start + end) / 2f;
            Vector3 apex = midPoint + Vector3.up * height;
            return new Vector3[] { start, apex, end };
        }

        public static T FindNearestEfficient<T> (this IEnumerable<T> source, T target) where T : Component
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            T nearest = default;
            float minDistance = float.MaxValue;
            bool found = false;

            foreach (var item in source)
            {
                float distance = Vector3.SqrMagnitude(item.transform.position - target.transform.position);

                if (distance < minDistance * minDistance)
                {
                    minDistance = distance;
                    nearest = item;
                    found = true;
                }
            }

            return found ? nearest : default;
        }
    }
}
