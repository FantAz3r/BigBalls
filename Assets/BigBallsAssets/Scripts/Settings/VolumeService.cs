using BigBalls.Services;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeService
{
    private const float MinVolume = 0.0001f;
    private const float UnitySliderValue = 0.8f;
    private const float MinDecibels = -80f;
    private const float MaxDecibels = 20f;

    private readonly AudioMixer _audioMixer;

    public VolumeService (IResourceLoader resourceLoader)
    {
        _audioMixer = resourceLoader.Load<GameConfig>().AudioMixer;
    }

    public void Apply (float sliderValue, string param)
    {
        float dB = ToDecibels(sliderValue);
        _audioMixer.SetFloat(param, dB);
    }

    private static float ToDecibels (float sliderValue)
    {
        float dB;
        if (sliderValue <= MinVolume)
            dB = MinDecibels;
        else if (sliderValue <= UnitySliderValue)
            dB = 20f * Mathf.Log10(sliderValue / UnitySliderValue);
        else
        {
            float t = (sliderValue - UnitySliderValue) / (1f - UnitySliderValue);
            dB = Mathf.Lerp(0f, MaxDecibels, t);
        }
        return Mathf.Clamp(dB, MinDecibels, MaxDecibels);
    }
}
