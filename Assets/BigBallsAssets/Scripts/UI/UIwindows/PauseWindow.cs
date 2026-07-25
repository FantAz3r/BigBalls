using BigBalls.Services;
using VContainer;

namespace BigBalls.UI
{
    public class PauseWindow : PopupWindow
    {
        private ITimeService _timeService;

        [Inject]
        public void Construct (ITimeService timeService)
        {
            _timeService = timeService;
        }

        public override void Open()
        {
            base.Open();
            _timeService.StopGame();
        }

        public override void Close()
        {
            base.Close();
            _timeService.ResumeGame();
        }
    }
}

