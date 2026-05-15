namespace BigBalls.GameplayObjects
{
    public class Stat
    {
        public readonly StatType StatType;

        public Stat(StatType statType, float value)
        {
            StatType = statType;
            Value = value;
        }

        public float Value { get; private set; }

        public void ChangeStat(float newValue)
        {
            Value = newValue;
        }
    }
}