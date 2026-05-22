using BigBalls.GameplayObjects;
using BigBalls.Providers;
using System.Collections.Generic;

namespace BigBalls.Services
{
    public class PlayerProvider : IPlayerProvider
    {
        public Player Player { get; private set; }

        public IReadOnlyList<Stat> Stats { get; private set; }

        public void Set(Player player) => Player = player;
        public void Set(List<Stat> stats) => Stats = stats;
    }
}
