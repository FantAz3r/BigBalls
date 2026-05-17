using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Services
{
    public class UpdateService : MonoBehaviour, IUpdateService
    {
        private List<IUpdateble> _tickables = new();
        private bool _isPaused = false;

        private void Update()
        {
            if (_isPaused) return;

            for (int i = _tickables.Count - 1; i >= 0; i--)
            {
                _tickables[i].Tick();
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

        public void Clear()
        {
            _tickables.Clear();
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
        }
    }
}
