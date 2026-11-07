using System;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigBalls.Services
{
    public class DamageService : IDamageService
    {
        private const float ArmorMultiplyer = 0.06f;
        private readonly IEntityRepository _entityRepository;
        private readonly DamageTextFactory _damageTextFactory;

        public DamageService (IEntityRepository entityRepository, DamageTextFactory damageTextFactory)
        {
            _entityRepository = entityRepository;
            _damageTextFactory = damageTextFactory;
        }

        public float ApplyDamage (IEntity target, float damage, Color color = default)
            => ApplyDamage(null, target, damage, color);

        public float ApplyDamage (IEntity self, IEntity target, float damage, Color color = default)
        {
            StatHolder statHolder = _entityRepository.GetStatHolder(target);

            if (statHolder == null)
                return 0;

            if (statHolder.Stats.ContainsKey(StatType.Evasion))
                if (ApplyEvasion(statHolder[StatType.Evasion]))
                {
                    _damageTextFactory.CreateText(target.Transform.position, TextLocalizator.Miss, Color.white);
                    return 0;
                }

            float finalDamage = damage;
            float actualDamage = 0;


            if (statHolder.Stats.ContainsKey(StatType.Armor))
            {
                finalDamage = ApplyArmor(statHolder[StatType.Armor], finalDamage);
            }

            if (statHolder.Stats.ContainsKey(StatType.Shield))
            {
                finalDamage = ApplyShield(statHolder[StatType.Shield], finalDamage);
            }

            if (statHolder.Stats.ContainsKey(StatType.Health))
            {
                actualDamage = ApplyHealthDamage(statHolder[StatType.Health], finalDamage, target, self);
            }

            if (actualDamage > 0)
            {
                _damageTextFactory.Create(target.Transform.position, actualDamage, color);
            }

            return actualDamage;
        }

        public float ApplySelfDamage (IEntity entity, float damage, Color color = default)
        {
            StatHolder statHolder = _entityRepository.GetStatHolder(entity);

            if (statHolder == null)
                return 0;

            float actualDamage = 0;

            if (statHolder.TryGetStat(out Stat health, StatType.Health))
            {
                actualDamage = health.SelfReduceCurrentValue(damage);
            }

            if (actualDamage > 0)
            {
                _damageTextFactory.Create(entity.Transform.position, actualDamage, color);
            }

            return actualDamage;
        }

        public float ApplyHeal (IEntity entity, float healValue, Color color)
        {
            StatHolder statHolder = _entityRepository.GetStatHolder(entity);

            if (statHolder == null)
                return 0;

            float finalHeal = healValue;
            float actualHeal = 0;

            if (statHolder.Stats.ContainsKey(StatType.Health))
            {
                actualHeal = ApplyHealHealth(statHolder[StatType.Health], finalHeal);
            }

            if (actualHeal > 0)
                _damageTextFactory.Create(entity.Transform.position, actualHeal, color);

            return actualHeal;
        }

        private bool ApplyEvasion (Stat evasion)
        {
            if (evasion.CurrentValue <= evasion.MinValue)
                return false;

            float evasionChance = evasion.CurrentValue;
            float roll = Random.Range(evasion.MinValue, evasion.MaxValue);

            return roll <= evasionChance;
        }

        private float ApplyArmor (Stat armor, float damage)
        {
            float a = armor.CurrentValue;

            if (Mathf.Approximately(a, 0f))
                return damage;

            float finalDamage = a > 0f
                ? damage / (1f + ArmorMultiplyer * a)
                : damage * (1f + ArmorMultiplyer * Mathf.Abs(a));

            return MathF.Max(0f, finalDamage);
        }

        private float ApplyShield (Stat shield, float damage)
        {
            if (shield.CurrentValue <= 0)
                return damage;

            float value = damage - shield.ReduceCurrentValue(damage);
            return value;
        }

        private float ApplyHealthDamage (Stat health, float damage, IEntity target = null, IEntity self = null)
        {
            if (damage <= 0)
                return 0;

            float redusedDamage = health.ReduceCurrentValue(damage);

            if (health.CurrentValue <= health.MinValue && target != null)
                self?.EventHandler.Kill(self, target);

            return redusedDamage;
        }

        private float ApplyHealHealth (Stat health, float healValue)
        {
            if (healValue <= 0)
                return 0;

            return health.AddCurrentValue(healValue);
        }
    }
}
