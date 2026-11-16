using UnityEngine;

namespace BigBalls.UI
{
    public class HUD : WindowBase
    {
        [field: SerializeField] public PlayerHealthView PlayerHealthViewer { get; private set; }
        [field: SerializeField] public PlayerExperienceViewer PlayerExperienceViewer { get; private set; }
        [field: SerializeField] public PlayerWalletViewer PlayerWalletViewer { get; private set; }
        [field: SerializeField] public PlayerLevelViewer PlayerLevelViewer { get; private set; }
        [field: SerializeField] public WaveViewer WaveViewer { get; private set; }
        [field: SerializeField] public PlayerSlots PlayerSlots { get; private set; }
        [field: SerializeField] public ComboViewer ComboViewer { get; private set; }
        [field: SerializeField] public ScoreViewer ScoreViewer { get; private set; }
    }
}
