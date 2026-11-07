public class SoundSettingsSaver
{
    private readonly ISaveService _saveService;

    public SoundSettingsSaver (ISaveService saveService) => _saveService = saveService;

    public float Load (string name, float fallback)
    {
        var progress = _saveService.GameProgress;
        var sound = progress?.Sounds?.Find(v => v.Name == name);
        return sound != null && !string.IsNullOrEmpty(sound.Name) ? sound.Volume : fallback;
    }

    public void Save (string name, float value)
    {
        var progress = _saveService.GameProgress;
        int index = progress.Sounds.FindIndex(v => v.Name == name);

        var data = new SoundSaveData { Name = name, Volume = value };

        if (index >= 0)
            progress.Sounds[index] = data;
        else
            progress.Sounds.Add(data);

        _saveService.Save();
    }
}
