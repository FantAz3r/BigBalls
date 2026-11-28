public sealed class SoundSettingsController
{
    private readonly VolumeService _volumeService;
    private readonly SoundSettingsSaver _storage;

    public SoundSettingsController (VolumeService volumeService, SoundSettingsSaver storage)
    {
        _volumeService = volumeService;
        _storage = storage;
    }

    public float Load (string param, float fallback)
        => _storage.Load(param, fallback);

    public void Apply (string param, float sliderValue)
        => _volumeService.Apply(sliderValue, param);

    public void Persist (string param, float sliderValue)
        => _storage.Save(param, sliderValue);

    public float LoadAndApply (string param, float fallback)
    {
        float saved = _storage.Load(param, fallback);
        _volumeService.Apply(saved, param);
        return saved;
    }

    public void ApplyAll ()
    {
        foreach (var sound in _storage.LoadAll())
            _volumeService.Apply(sound.Volume, sound.Name);
    }
}