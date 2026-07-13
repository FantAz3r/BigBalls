using VContainer;

public class AutoAttackSetting : SwitchHandler
{
    private PlayerAttackService _playerAttackService;

    [Inject]
    public void Construct(PlayerAttackService playerAttackService)
    {
        _playerAttackService = playerAttackService;
    }

    protected override void OnClick(bool isOn)
    {
        _playerAttackService.AutoAttack(isOn);
    }
}
