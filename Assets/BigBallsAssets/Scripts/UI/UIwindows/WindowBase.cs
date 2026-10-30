using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.UI
{
    public class WindowBase : MonoBehaviour, IWindow
    {
        private IInputService _inputService;

        [Inject]
        public void Construct(InputServiceAdapter inputServiceAdapter)
        {
            _inputService = inputServiceAdapter.Get();
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            _inputService.DisablePlayer();
            _inputService.DisableUI();
        }

        public virtual void Close()
        {
            _inputService.EnablePlayer();
            _inputService.EnableUI();
            gameObject.SetActive(false);
        }
    }
}

