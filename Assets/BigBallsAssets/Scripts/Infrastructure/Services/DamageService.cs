using BigBalls.GameplayObjects;
using System;
using Random = UnityEngine.Random;

namespace BigBalls.Services
{
    public class DamageService : IDamageService
    {
        private const float ArmorMultiplyer = 0.06f;
        private readonly IEntityRepository _entityRepository;
        private readonly DamageTextFactory _damageTextFactory;

        public DamageService(IEntityRepository entityRepository, DamageTextFactory damageTextFactory)
        {
            _entityRepository = entityRepository;
            _damageTextFactory = damageTextFactory;
        }

        public float ApplyDamage(IEntity entity, float damage)
        {
            StatHolder statHolder = _entityRepository.GetStatHolder(entity);

            if (statHolder == null)
                return 0;

            if (statHolder.Stats.ContainsKey(StatType.Evasion))
                if (ApplyEvasion(statHolder[StatType.Evasion]))
                    return 0;

            float finalDamage = damage;

            if (statHolder.Stats.ContainsKey(StatType.Armor))
            {
                finalDamage = ApplyArmor(statHolder[StatType.Armor], finalDamage);
            }

            if (statHolder.Stats.ContainsKey(StatType.Health))
            {
                ApplyHealthDamage(statHolder[StatType.Health], finalDamage);
            }

            _damageTextFactory.Create(entity.Transform.position, finalDamage);
            return finalDamage;
        }

        private bool ApplyEvasion(Stat evasion)
        {
            if (evasion.CurrentValue <= evasion.MinValue)
                return false;

            float evasionChance = evasion.CurrentValue;
            float roll = Random.Range(evasion.MinValue, evasion.MaxValue);

            return roll <= evasionChance;
        }

        private float ApplyArmor(Stat armor, float damage)
        {
            if (armor.CurrentValue <= 0) return damage;

            float armorValue = armor.CurrentValue;
            float damageReduction = (ArmorMultiplyer * armorValue) / (1 + ArmorMultiplyer * armorValue);
            float finalDamage = damage * (1f - damageReduction);

            return MathF.Max(0f, finalDamage);
        }

        private void ApplyHealthDamage(Stat health, float damage)
        {
            if (damage <= 0)
                return;

            health.ReduceCurrentValue(damage);
        }
    }
}
