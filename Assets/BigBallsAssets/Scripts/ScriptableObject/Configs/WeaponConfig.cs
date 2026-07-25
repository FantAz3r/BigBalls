using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/Item/WeaponConfigs")]

    public class WeaponConfig : ItemConfig, IWeapon
    {
        [SerializeField] private List<BallConfig> _uniqueBalls;

        [field: SerializeField] public Cannon Cannon { get; set; }

        public List<BallConfig> UniqueBallConfigs => _uniqueBalls;

        public override List<BallConfig> GetBalls (int level)
        {
            return _uniqueBalls;
        }
    }
}