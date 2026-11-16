using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using BigBalls.Infrastructure;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]

public class GameConfig : ScriptableObject
{
    [Header("Sounds")]
    public AudioMixer AudioMixer;

    [Header("Scores")]
    public int BaseKillScore = 10;
    public int MaxCombo = 25;
    [Range(1f, 3f)] public float ComboKillBonus = 1.15f;
    public float ComboWindowSeconds = 3f;
    public int BaseLevelScore = 100;
    public float LevelGrowth = 1.2f;
    public List<float> StarMyltyplyers = new();


    [Header("Liderboards")]
    public string MainLiderboard;
    public SerializedDictionary<LevelID, string> Liderboards = new();

    public string GetLiderbord (LevelID level) => Liderboards[level];
}
