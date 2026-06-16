using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/Item/WeaponConfigs")]

    public class WeaponConfig : ItemConfig, IWeapon
    {
        [SerializeField] private List<BallModel> _uniqueBalls;

        public List<BallModel> UniqueBalls => _uniqueBalls;
    }
}