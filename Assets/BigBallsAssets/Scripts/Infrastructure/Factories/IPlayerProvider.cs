using BigBalls.GameplayObjects;
using System.Collections.Generic;

namespace BigBalls.Providers
{
    public interface IPlayerProvider
    {
        Player Player { get; }
        IReadOnlyList<Stat> Stats { get; }
        void Set(Player player);
        void Set(List<Stat> stats);
    }
}