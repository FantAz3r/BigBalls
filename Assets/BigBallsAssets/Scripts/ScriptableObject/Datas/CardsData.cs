using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.StaticData;
using UnityEngine;

public class CardsData : ScriptableObject
{
    private const string LogTag = "[CardsData]";

    [SerializeField] private List<ArtefactConfig> _artefacts = new();
    [SerializeField] private List<BallConfig> _balls = new();
    [SerializeField] private List<ArmorConfig> _armors = new();
    [SerializeField] private List<WeaponConfig> _weapons = new();
    [SerializeField] private List<HelmetConfig> _helmets = new();
    [SerializeField] private List<BallConfig> _enemyProjectiles = new();

    public Dictionary<ArtefactType, ArtefactConfig> Artefacts => Build("Artefacts", _artefacts, c => c.ArtefactType);
    public Dictionary<BallType, BallConfig> Balls => Build("Balls", _balls, c => c.BallType);
    public Dictionary<ItemType, ArmorConfig> Armors => Build("Armors", _armors, c => c.ItemType);
    public Dictionary<ItemType, WeaponConfig> Weapons => Build("Weapons", _weapons, c => c.ItemType);
    public Dictionary<ItemType, HelmetConfig> Helmets => Build("Helmets", _helmets, c => c.ItemType);
    public Dictionary<BallType, BallConfig> EnemyProjectiles => Build("EnemyProjectiles", _enemyProjectiles, c => c.BallType);

    private static Dictionary<TKey, TConfig> Build<TKey, TConfig> (
        string name,
        List<TConfig> list,
        System.Func<TConfig, TKey> keySelector)
        where TKey : System.Enum
        where TConfig : Object
    {
        if (list == null)
            return new Dictionary<TKey, TConfig>();

        var map = new Dictionary<TKey, TConfig>(list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            var config = list[i];

            if (config == null)
                continue;

            var key = keySelector(config);

            if (map.TryAdd(key, config) == false)
            {
                var existing = map[key];
            }
        }
        return map;
    }
}