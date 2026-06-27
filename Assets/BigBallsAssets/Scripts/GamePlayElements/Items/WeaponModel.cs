using BigBalls.Configs;
using BigBalls.StaticData;
using System.Collections.Generic;

public class WeaponModel : ItemModel, IWeapon
{
    public WeaponModel(int id, ItemConfig config, int level = 0) : base(id, config, level)
    {
    }

    public WeaponConfig WeaponConfig { get; private set; }
    public List<BallModel> UniqueBalls => WeaponConfig.UniqueBalls;
}
