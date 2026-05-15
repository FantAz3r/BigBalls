using BigBalls.Services;
using UnityEngine;

namespace BigBalls.UI
{
    public class PauseWindow : WindowBase
    {
        private const float ViewDuration = 0.5f;
        private const float TargetScale = 1f;

        private ITimeService _timeService;

        public void Construct(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public override void Open()
        {
            base.Open();
            _timeService.StopGame();

            transform.localScale = Vector3.zero;
        }

        public override void Close()
        {
            base.Close();
            _timeService.ResumeGame();
            Destroy(gameObject);
        }
    }
}

