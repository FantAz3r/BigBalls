using BigBalls.Providers;
using UnityEngine;
using VContainer;

namespace BigBalls.UI
{
    public class HUD : WindowBase
    {
        [field: SerializeField] public PlayerHealthView PlayerHealthViewer { get; private set; }

        public override void Open()
        {
            base.Open();
        }
    }
}
