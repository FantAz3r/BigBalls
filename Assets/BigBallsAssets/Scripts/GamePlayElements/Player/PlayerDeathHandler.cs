namespace BigBalls.GameplayObjects
{
    public class PlayerDeathHandler
    {
        private readonly Player _player;
        private readonly Stat _health;

        public PlayerDeathHandler(Player player, Stat health)
        {
            _player = player;
            _health = health;

            _health.ValueChanged += HandleDeath;
        }

        private void HandleDeath(IReadonlyStat stat)
        {
            if (stat.CurrentValue <= stat.MinValue)
            {
                //Die
            }
        }



        public void Dispose()
        {
            _health.ValueChanged -= HandleDeath;
        }
    }
}

