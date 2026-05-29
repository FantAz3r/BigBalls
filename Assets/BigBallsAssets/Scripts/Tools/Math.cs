using System.Collections.Generic;

namespace Utils
{
    public static class Math 
    {
        public static List<int[]> GetPositionCombinations(int n, int k) => GetPositionCombinationsOrPermutations(n, k, false);
        public static List<int[]> GetPositionPermutations(int n, int k) => GetPositionCombinationsOrPermutations(n, k, true);

        private static List<int[]> GetPositionCombinationsOrPermutations(int n, int k, bool usePermutations)
        {
            var results = new List<int[]>();

            void Backtrack(List<int> current, HashSet<int> used, int start)
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

    }
}
