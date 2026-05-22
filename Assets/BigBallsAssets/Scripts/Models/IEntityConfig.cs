using System.Collections.Generic;

namespace BigBalls.StaticData
{
    public interface IEntityConfig
    {
        IEnumerable<StatStruct> Stats { get; }
    }
}