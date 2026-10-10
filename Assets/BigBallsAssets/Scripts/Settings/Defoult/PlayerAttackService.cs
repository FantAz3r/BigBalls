public class PlayerAttackService
{
    public bool IsAutoAttack { get; private set; } = false;

    public void AutoAttack(bool isOn)
    {
        IsAutoAttack = isOn;
    }
}