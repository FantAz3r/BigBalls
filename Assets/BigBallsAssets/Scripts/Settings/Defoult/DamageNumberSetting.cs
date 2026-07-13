using VContainer;

public class DamageNumberSetting : SwitchHandler
{
    private ISaveService _saveService;

    [Inject]
    public void Construct( ISaveService saveService)
    {
        _saveService = saveService;
    }

    private void Start()
    {
        Switch.IsOn = _saveService.GameProgress.ShowDamageNumbers;
    }

    protected override void OnClick(bool isOn)
    {
        _saveService.GameProgress.ShowDamageNumbers = isOn;
        _saveService.Save();
    }
}
