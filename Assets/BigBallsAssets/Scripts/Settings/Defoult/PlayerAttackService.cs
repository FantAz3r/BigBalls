public class PlayerAttackService
{
    public bool IsAutoAttack { get; private set; } = true;

    public void AutoAttack(bool isOn)
    {
        IsAutoAttack = isOn;
    }
}