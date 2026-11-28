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
    }

    private void OnDisable ()
    {
        if (_volumeSlider == null)
            return;

        _volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
        _controller.Persist(_param, _volumeSlider.value);
    }

    private void OnSliderChanged (float value)
    {
        if (_suppressCallback)
            return;

        _controller.Apply(_param, value);
    }
}