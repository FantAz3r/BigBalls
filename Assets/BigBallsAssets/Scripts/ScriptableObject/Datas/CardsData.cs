using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using BigBalls.Configs;
using BigBalls.StaticData;
using UnityEngine;

[CreateAssetMenu(menuName = "Datas/CardData")]
public class CardsData : ScriptableObject
{
    [SerializeField] private SerializedDictionary<ArtefactType, ArtefactConfig> _artefacts = new ();
    [SerializeField] private SerializedDictionary<BallType, BallConfig> _balls = new ();

    public Dictionary<ArtefactType , ArtefactConfig> Artefacts => _artefacts;
    public Dictionary<BallType, BallConfig> Balls => _balls;
}
