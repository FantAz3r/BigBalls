using AYellowpaper.SerializedCollections;
using BigBalls.Configs;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Datas/CardData")]
public class CardsData : ScriptableObject
{
    [SerializeField] private SerializedDictionary<ArtefactType, ArtefactConfig> _artefacts = new();
    [SerializeField] private SerializedDictionary<BallType, BallConfig> _balls = new();
    [SerializeField] private SerializedDictionary<ItemType, ArmorConfig> _armors = new();
    [SerializeField] private SerializedDictionary<ItemType, WeaponConfig> _weapons = new();
    [SerializeField] private SerializedDictionary<ItemType, HelmetConfig> _helmets = new();

    public Dictionary<ArtefactType, ArtefactConfig> Artefacts => _artefacts;
    public Dictionary<BallType, BallConfig> Balls => _balls;
    public Dictionary<ItemType, ArmorConfig> Armors => _armors;
    public Dictionary<ItemType, WeaponConfig> Weapons => _weapons;
    public Dictionary<ItemType, HelmetConfig> Helmets => _helmets;

}
