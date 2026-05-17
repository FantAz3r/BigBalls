using VContainer.Unity;
using BigBalls.Services;
using BigBalls.UI;

namespace BigBalls.Infrastructure
{
    public class MenuEntryPoint: IStartable
    {
        private readonly IWindowService _windowService;
        public MenuEntryPoint(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public void Start()
        {
            _windowService.CreateUIRoot();
            _windowService.Open(WindowType.Background);
            _windowService.Open(WindowType.MainMenu);
        }
    }
}