using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/Item/WeaponConfigs")]

    public class WeaponConfig : ItemConfig, IWeapon
    {
        [SerializeField] private List<BallConfig> _uniqueBalls;

        public List<BallConfig> UniqueBalls => _uniqueBalls;
    }
}