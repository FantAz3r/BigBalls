using System.Drawing;
using BigBalls.GameplayObjects;

namespace BigBalls.Services
{
    public interface IDamageService
    {
        float ApplyDamage (IEntity entity, float damage, UnityEngine.Color color = default);
    }
}