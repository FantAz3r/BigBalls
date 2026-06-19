using UnityEngine;

namespace BigBalls.UI
{
    public class HUD : WindowBase
    {
        [field: SerializeField] public PlayerHealthView PlayerHealthViewer { get; private set; }
        [field: SerializeField] public PlayerExperienceViewer PlayerExperienceViewer { get; private set; }
        [field: SerializeField] public WaveViewer WaveViewer { get; private set; }

        public override void Open()
        {
            base.Open();
        }
    }
}
