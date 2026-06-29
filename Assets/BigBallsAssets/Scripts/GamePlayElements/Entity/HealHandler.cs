using BigBalls.GameplayObjects;

public class HealHandler
{
    private Stat _health;
    
    public HealHandler(Stat health)
    {
        _health = health;
    }

    public void Heal(float value)
    {
        _health.AddCurrentValue(value);
    }
}