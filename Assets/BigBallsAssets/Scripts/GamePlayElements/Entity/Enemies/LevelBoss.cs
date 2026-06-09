using System;

namespace BigBalls.GameplayObjects
{
    public class LevelBoss : Boss, IWinReason
    {
        public event Action Won;


        private void OnDisable()
        {
            Won?.Invoke();
        }
    }
}
