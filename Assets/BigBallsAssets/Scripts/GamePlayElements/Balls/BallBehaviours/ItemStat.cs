namespace BigBalls.GameplayObjects
{
    public struct ItemStat
    {
        public string Name;
        public float Value;
        public float NextValue;
        public bool IsPercent;

        public ItemStat(string name, float value, float nextValue = 0, bool isPercent = false)
        {
            Name = name;
            Value = value;
            NextValue = nextValue;
            IsPercent = isPercent;
        }
    }
}