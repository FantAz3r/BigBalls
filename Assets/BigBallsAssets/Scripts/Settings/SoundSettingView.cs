using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SoundSettingView : MonoBehaviour
{
    private const float DefaultSliderValue = 0.5f;

    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private string _param;

    private SoundSettingsController _controller;
    private bool _suppressCallback;

    [Inject]
    public void Construct (SoundSettingsController controller)
    {
        _controller = controller;
    }

    private void OnEnable ()
    {
        if (_volumeSlider == null) return;

        _volumeSlider.onValueChanged.AddListener(OnSliderChanged);

        float saved = _controller.Load(_param, DefaultSliderValue);

        _suppressCallback = true;
        _volumeSlider.value = saved;
        _suppressCallback = false;

        _controller.Apply(_param, saved);
    }

    private void OnDisable ()
    {
        if (_volumeSlider == null) return;

        _volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);

        _controller.Persist(_param, _volumeSlider.value);
    }

    private void OnSliderChanged (float value)
    {
        if (_suppressCallback) return;

        _controller.Apply(_param, value);
    }
}

//private const float DefaultSliderValue = 0.5f;
//
//    [SerializeField] private AudioMixer _audioMixer;
//[SerializeField] private List<string> _volumeParameters;
//
//private float _minVolume = 0.0001f;
//private float _minDecibels = -80f;
//private float _maxDecibels = 20f;
//private Slider _volumeSlider;
//private float _currentValue;
//private float _lastSavedSliderValue;
//private float _unitySliderValue = 0.8f;
//
//private ISaveService _saveService;
//
//[Inject]
//public void Construct (ISaveService saveService)
//{
//    _saveService = saveService;
//    _volumeSlider = GetComponent<Slider>();
//    LoadVolume();
//    _volumeSlider.onValueChanged.AddListener(SetVolume);
//}
//
//private void OnDestroy ()
//{
//    if (_volumeSlider != null)
//    {
//        _volumeSlider.onValueChanged.RemoveListener(SetVolume);
//    }
//}
//
//public void SetVolume (float sliderValue)
//{
//    float dB;
//
//    if (sliderValue <= _minVolume)
//    {
//        dB = _minDecibels;
//    }
//    else if (sliderValue <= _unitySliderValue)
//    {
//        dB = 20f * Mathf.Log10(sliderValue / _unitySliderValue);
//    }
//    else
//    {
//        float t = (sliderValue - _unitySliderValue) / (1f - _unitySliderValue);
//        dB = Mathf.Lerp(0f, _maxDecibels, t);
//    }
//
//    dB = Mathf.Clamp(dB, _minDecibels, _maxDecibels);
//
//    foreach (var param in _volumeParameters)
//    {
//        _audioMixer.SetFloat(param, dB);
//    }
//
//    _lastSavedSliderValue = sliderValue;
//    SaveVolume();
//}
//
//private void SaveVolume ()
//{
//    var progress = _saveService.GameProgress;
//
//    foreach (var volumeParam in _volumeParameters)
//    {
//        int index = progress.Sounds.FindIndex(v => v.Name == volumeParam);
//
//        if (index >= 0)
//        {
//            progress.Sounds[index] = new SoundSaveData
//            {
//                Name = volumeParam,
//                Volume = _lastSavedSliderValue
//            };
//        }
//        else
//        {
//            progress.Sounds.Add(new SoundSaveData
//            {
//                Name = volumeParam,
//                Volume = _lastSavedSliderValue
//            });
//        }
//    }
//
//    _saveService.Save();
//}
//
//private void LoadVolume ()
//{
//    var progress = _saveService.GameProgress;
//
//    if (progress == null || progress.Sounds == null || _volumeParameters.Count == 0)
//    {
//        _volumeSlider.value = DefaultSliderValue;
//        SetVolume(_volumeSlider.value);
//        return;
//    }
//
//    SoundSaveData soundData = progress.Sounds.Find(v => v.Name == _volumeParameters[0]);
//
//    if (soundData != null && !string.IsNullOrEmpty(soundData.Name))
//    {
//        float loadedSliderValue = soundData.Volume;
//        _volumeSlider.value = loadedSliderValue;
//        _lastSavedSliderValue = loadedSliderValue;
//        SetVolume(_volumeSlider.value);
//    }
//    else
//    {
//        _volumeSlider.value = DefaultSliderValue;
//        SetVolume(_volumeSlider.value);
//    }
//}