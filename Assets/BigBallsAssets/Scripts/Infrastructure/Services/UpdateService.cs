using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace BigBalls.Services
{
    public class UpdateService : MonoBehaviour, IUpdateService
    {
        private List<IUpdateble> _tickables = new();
        private List<IUpdateble> _fixedTickables = new();
        private ITimeService _timeService;
        private bool _isPaused = false;

        [Inject]
        public void Construct(ITimeService timeService)
        {
            _timeService = timeService;
        }

        private void Update()
        {
            if (_timeService != null && _timeService.IsPaused)
                return;

            for (int i = _tickables.Count - 1; i >= 0; i--)
            {
                _tickables[i]?.Tick();
            }
        }

        private void FixedUpdate()
        {
            if (_timeService != null && _timeService.IsPaused)
                return;

            if (_fixedTickables.Count == 0)
                return;

            for (int i = 0; i < _fixedTickables.Count - 1; i++)
            {
                _fixedTickables[i]?.Tick();
            }
        }

        public void Register(IUpdateble tickable)
        {
            _tickables.Add(tickable);
        }

        public void Unregister(IUpdateble tickable)
        {
            _tickables.Remove(tickable);
        }

        public void RegisterFixed(IUpdateble tickable)
        {
            _fixedTickables.Add(tickable);
        }

        public void UnregisterFixed(IUpdateble tickable)
        {
            _fixedTickables.Remove(tickable);
        }

        public void Clear()
        {
            _tickables.Clear();
            _fixedTickables.Clear();
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
        }
    }
}
