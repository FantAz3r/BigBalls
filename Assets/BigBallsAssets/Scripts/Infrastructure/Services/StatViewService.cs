using BigBalls.StaticData;
using System.Collections.Generic;

public class StatViewService
{
    Dictionary<string, float> _variables = new();


    public void Add(string name, StatStruct stat)
    {
        _variables[name + ".min"] = stat.MinValue;
        _variables[name + ".current"] = stat.StartCurrentValue;
        _variables[name + ".max"] = stat.StartMaxValue;
    }

    public float GetValue(string key) => _variables[key];
}
