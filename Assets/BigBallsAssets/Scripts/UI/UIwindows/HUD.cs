using UnityEngine;
using UnityEngine.Serialization;

namespace BigBalls.UI
{
    public class HUD : WindowBase
    {
        [field: SerializeField] public PlayerHealthView PlayerHealthViewer { get; private set; }
        [field: SerializeField] public PlayerExperienceViewer PlayerExperienceViewer { get; private set; }
        [field: SerializeField] public PlayerWalletViewer PlayerWalletViewer { get; private set; }
        [field: SerializeField] public PlayerLevelViewer PlayerLevelViewer { get; private set; }
        
        [field: SerializeField] public WaveViewer WaveViewer { get; private set; }
        [field: SerializeField] public CardSelectionMenu CardSelectionMenu { get; private set; }

        public override void Open()
        {
            base.Open();
        }
    }
}
