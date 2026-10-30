using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]

public class GameConfig : ScriptableObject
{
    public AudioMixer AudioMixer;
}
