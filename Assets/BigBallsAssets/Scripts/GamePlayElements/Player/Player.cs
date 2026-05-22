using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public PlayerConfig PlayerConfig { get; private set; }
        [field: SerializeField] public ResourceCollector ResourceCollector { get; private set; }
        public int ID { get; private set; }

        //public Inventory Inventory { get; private set; }
        //public PlayerExperience Experience { get; private set; }
        //public PlayerCardConfigContainer CardHolder { get; private set; }
        //public Health Health { get; private set; }
        //public Mover Mover { get; private set; }
        //public Rotator Rotator { get; private set; }
        //public HealthRegenerator HealthRegeneration { get; private set; }
        
        public PlayerAnimator PlayerAnimator { get; private set; }

        public void Construct(int id, PlayerAnimator playerAnimator)
        {
            ID = id;
            PlayerAnimator = playerAnimator;
        }
    }
}
