using System.Drawing;
using BigBalls.GameplayObjects;

namespace BigBalls.Services
{
    public interface IDamageService
    {
        float ApplyDamage (IEntity entity, float damage, UnityEngine.Color color = default);
        float ApplySelfDamage(IEntity entity, float damage, UnityEngine.Color color = default);
        float ApplyHeal (IEntity entity, float healValue, UnityEngine.Color color);
        float ApplyDamage (IEntity self, IEntity target, float damage, UnityEngine.Color color = default);
    }
}