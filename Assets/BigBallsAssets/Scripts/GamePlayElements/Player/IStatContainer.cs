using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public interface IStatContainer
    {
        IReadOnlyList<Stat> Stats { get; }
    }
}