using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Datas/Sounds")]

public class SoundData : ScriptableObject
{
    [SerializeField] private SerializedDictionary<SoundType, SoundStruct> _sounds = new();

    public AudioMixerGroup EffectGroup;
    public AudioMixerGroup MusicGroup;

    public SoundStruct this[SoundType name] => _sounds[name];
}
