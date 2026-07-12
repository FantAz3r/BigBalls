namespace BigBalls.GameplayObjects
{
    public struct ItemStat
    {
        public string Name;
        public float Value;
        public float NextValue;

        public ItemStat(string name, float value, float nextValue)
        {
            Name = name;
            Value = value;
            NextValue = nextValue;
        }
    }
}