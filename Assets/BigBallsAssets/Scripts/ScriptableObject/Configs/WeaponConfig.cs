using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/Item/WeaponConfigs")]

    public class WeaponConfig : ItemConfig, IWeapon
    {
        [SerializeField] private List<BallStruct> _uniqueBalls;

        public List<BallStruct> UniqueBalls => _uniqueBalls;
    }
}