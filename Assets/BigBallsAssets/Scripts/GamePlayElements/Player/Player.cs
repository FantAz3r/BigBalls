using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Player : MonoBehaviour, IStatContainer
    {
        [field: SerializeField] public PlayerConfig PlayerConfig { get; private set; }
        [field: SerializeField] public ResourceCollector ResourceCollector { get; private set; }

        //public Inventory Inventory { get; private set; }
        //public PlayerExperience Experience { get; private set; }
        //public PlayerCardConfigContainer CardHolder { get; private set; }
        //public Health Health { get; private set; }
        //public Mover Mover { get; private set; }
        //public Rotator Rotator { get; private set; }
        //public HealthRegenerator HealthRegeneration { get; private set; }

        private List<Stat> _statHolder = new();
        
        public IReadOnlyList<Stat> Stats => _statHolder.AsReadOnly();
        public PlayerAnimator PlayerAnimator { get; private set; }


        public void Construct(PlayerAnimator playerAnimator)
        {
            PlayerAnimator = playerAnimator;
            InitStats();
        }

        private void InitStats()
        {
            foreach (var stat in PlayerConfig.Stats)
            {
                _statHolder.Add(new Stat(stat.Key, stat.Value));

            }
        }
    }
}
