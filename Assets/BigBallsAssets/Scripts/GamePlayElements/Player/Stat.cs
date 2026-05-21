namespace BigBalls.GameplayObjects
{
    public class Stat
    {
        public readonly StatType Type;

        public Stat(StatType statType, float value)
        {
            Type = statType;
            Value = value;
        }

        public float Value { get; private set; }

        public void ChangeStat(float newValue)
        {
            Value = newValue;
        }
    }
}