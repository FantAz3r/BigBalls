using System;

namespace BigBalls.GameplayObjects
{
    public class LevelBoss : Boss, IWinReason
    {
        public event Action Won;


        protected virtual void OnDisable()
        {
            Won?.Invoke();
        }
    }
}
