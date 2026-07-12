using BigBalls.GameplayObjects;
using BigBalls.UI;
using System.Collections.Generic;
using YG;

namespace BigBalls.Localization
{
    public static class TextLocalizator
    {
        private const string Russian = "ru";
        private const string English = "en";
        private const string Turkish = "tr";

        private static readonly Dictionary<EntityType, Dictionary<string, string>> EntityTypeDict =
            new Dictionary<EntityType, Dictionary<string, string>>
            {
                {
                    EntityType.None, new Dictionary<string, string>
                    {
                        { Russian, "нет" },
                        { English, "none" },
                        { Turkish, "yok" },
                    }
                },
                {
                    EntityType.Enemy, new Dictionary<string, string>
                    {
                        { Russian, "враг" },
                        { English, "enemy" },
                        { Turkish, "düşman" },
                    }
                },
                {
                    EntityType.UniqueBall, new Dictionary<string, string>
                    {
                        { Russian, "уникальный шар" },
                        { English, "unique ball" },
                        { Turkish, "benzersiz top" },
                    }
                },
                {
                    EntityType.Player, new Dictionary<string, string>
                    {
                        { Russian, "игрок" },
                        { English, "player" },
                        { Turkish, "oyuncu" },
                    }
                },
                {
                    EntityType.Boss, new Dictionary<string, string>
                    {
                        { Russian, "босс" },
                        { English, "boss" },
                        { Turkish, "şef" },
                    }
                },
                {
                    EntityType.LevelBoss, new Dictionary<string, string>
                    {
                        { Russian, "финальный босс" },
                        { English, "final boss" },
                        { Turkish, "seviye şefi" },
                    }
                },
                {
                    EntityType.BaseBall, new Dictionary<string, string>
                    {
                        { Russian, "базовый шар" },
                        { English, "base ball" },
                        { Turkish, "temel top" },
                    }
                },
            };

        private static readonly Dictionary<StatType, Dictionary<string, string>> StatTypeDict =
            new Dictionary<StatType, Dictionary<string, string>>
            {
                {
                    StatType.None, new Dictionary<string, string>
                    {
                        { Russian, "нет" },
                        { English, "none" },
                        { Turkish, "yok" },
                    }
                },
                {
                    StatType.Health, new Dictionary<string, string>
                    {
                        { Russian, "здоровье" },
                        { English, "health" },
                        { Turkish, "sağlık" },
                    }
                },
                {
                    StatType.Damage, new Dictionary<string, string>
                    {
                        { Russian, "урон" },
                        { English, "damage" },
                        { Turkish, "hasar" },
                    }
                },
                {
                    StatType.MoveSpeed, new Dictionary<string, string>
                    {
                        { Russian, "скорость движения" },
                        { English, "move speed" },
                        { Turkish, "hareket hızı" },
                    }
                },
                {
                    StatType.RotationSpeed, new Dictionary<string, string>
                    {
                        { Russian, "скорость вращения" },
                        { English, "rotation speed" },
                        { Turkish, "dönme hızı" },
                    }
                },
                {
                    StatType.Armor, new Dictionary<string, string>
                    {
                        { Russian, "броня" },
                        { English, "armor" },
                        { Turkish, "zırh" },
                    }
                },
                {
                    StatType.HealthRegen, new Dictionary<string, string>
                    {
                        { Russian, "регенерация здоровья" },
                        { English, "health regen" },
                        { Turkish, "can yenilenmesi" },
                    }
                },
                {
                    StatType.Evasion, new Dictionary<string, string>
                    {
                        { Russian, "уворот" },
                        { English, "evasion" },
                        { Turkish, "kaçınma" },
                    }
                },
                {
                    StatType.AttackSpeed, new Dictionary<string, string>
                    {
                        { Russian, "скорость атаки" },
                        { English, "attack speed" },
                        { Turkish, "saldırı hızı" },
                    }
                },
                {
                    StatType.BallBag, new Dictionary<string, string>
                    {
                        { Russian, "размер сумки" },
                        { English, "ball bag size" },
                        { Turkish, "top çantası" },
                    }
                },
                {
                    StatType.Experience, new Dictionary<string, string>
                    {
                        { Russian, "опыт" },
                        { English, "experience" },
                        { Turkish, "deneyim" },
                    }
                },
            };

        private static readonly Dictionary<WindowType, Dictionary<string, string>> WindowTypeDict =
            new Dictionary<WindowType, Dictionary<string, string>>
            {
                {
                    WindowType.None, new Dictionary<string, string>
                    {
                        { Russian, "нет" },
                        { English, "none" },
                        { Turkish, "yok" },
                    }
                },
                {
                    WindowType.Shop, new Dictionary<string, string>
                    {
                        { Russian, "магазин" },
                        { English, "shop" },
                        { Turkish, "dükkan" },
                    }
                },
                {
                    WindowType.WinLevelMenu, new Dictionary<string, string>
                    {
                        { Russian, "меню победы" },
                        { English, "win level menu" },
                        { Turkish, "seviye kazanma menüsü" },
                    }
                },
                {
                    WindowType.StartLevelMenu, new Dictionary<string, string>
                    {
                        { Russian, "меню начала уровня" },
                        { English, "start level menu" },
                        { Turkish, "seviye başlatma menüsü" },
                    }
                },
                {
                    WindowType.LouseLevelMenu, new Dictionary<string, string>
                    {
                        { Russian, "меню поражения" },
                        { English, "lose level menu" },
                        { Turkish, "seviye kaybetme menüsü" },
                    }
                },
                {
                    WindowType.Settings, new Dictionary<string, string>
                    {
                        { Russian, "настройки" },
                        { English, "settings" },
                        { Turkish, "ayarlar" },
                    }
                },
                {
                    WindowType.Pause, new Dictionary<string, string>
                    {
                        { Russian, "пауза" },
                        { English, "pause" },
                        { Turkish, "duraklat" },
                    }
                },
                {
                    WindowType.CardMenu, new Dictionary<string, string>
                    {
                        { Russian, "меню карт" },
                        { English, "card menu" },
                        { Turkish, "kart menüsü" },
                    }
                },
                {
                    WindowType.HUD, new Dictionary<string, string>
                    {
                        { Russian, "интерфейс" },
                        { English, "HUD" },
                        { Turkish, "gösterge paneli" },
                    }
                },
                {
                    WindowType.MainMenu, new Dictionary<string, string>
                    {
                        { Russian, "главное меню" },
                        { English, "main menu" },
                        { Turkish, "ana menü" },
                    }
                },
                {
                    WindowType.Background, new Dictionary<string, string>
                    {
                        { Russian, "фон" },
                        { English, "background" },
                        { Turkish, "arka plan" },
                    }
                },
                {
                    WindowType.WaveViewer, new Dictionary<string, string>
                    {
                        { Russian, "отображатель волн" },
                        { English, "wave viewer" },
                        { Turkish, "dalga görüntüleyici" },
                    }
                },
                {
                    WindowType.Joystick, new Dictionary<string, string>
                    {
                        { Russian, "джойстик" },
                        { English, "joystick" },
                        { Turkish, "joystick" },
                    }
                },
                {
                    WindowType.Inventory, new Dictionary<string, string>
                    {
                        { Russian, "Инвентарь" },
                        { English, "Inventory" },
                        { Turkish, "Envanter" },
                    }
                },
                {
                    WindowType.LeaderBoard, new Dictionary<string, string>
                    {
                        { Russian, "Таблица Лидеров" },
                        { English, "Leaderboard" },
                        { Turkish, "Liderlik Tablosu" },
                    }
                },
            };

        private static readonly Dictionary<string, string> DamageDict = new Dictionary<string, string>
        {
            { Russian, "урон" },
            { English, "damage" },
            { Turkish, "hasar" },
        };

        private static readonly Dictionary<string, string> AttackDelayDict = new Dictionary<string, string>
        {
            { Russian, "задержка атаки" },
            { English, "attack delay" },
            { Turkish, "saldırı gecikmesi" },
        };

        private static readonly Dictionary<string, string> BurnDPSDict = new Dictionary<string, string>
        {
            { Russian, "урон от горения" },
            { English, "burn damage per second" },
            { Turkish, "yanma hasarı saniye başına" },
        };

        private static readonly Dictionary<string, string> BurnDurationDict = new Dictionary<string, string>
        {
            { Russian, "время горения" },
            { English, "burn duration" },
            { Turkish, "yanma süresi" },
        };

        private static readonly Dictionary<string, string> FreezeDurationDict = new Dictionary<string, string>
        {
            { Russian, "время заморозки" },
            { English, "freeze duration" },
            { Turkish, "donma süresi" },
        };

        private static readonly Dictionary<string, string> SlowPercentDict = new Dictionary<string, string>
        {
            { Russian, "процент замедления" },
            { English, "slow percentage" },
            { Turkish, "yavaşlatma yüzdesi" },
        };

        private static readonly Dictionary<string, string> AttackRangeDict = new Dictionary<string, string>
        {
            { Russian, "дальность атаки" },
            { English, "attack range" },
            { Turkish, "saldırı menzili" },
        };

        private static readonly Dictionary<string, string> FlightDistanceDict = new Dictionary<string, string>
        {
            { Russian, "дальность полёта" },
            { English, "flight distance" },
            { Turkish, "uçuş mesafesi" },
        };

        private static readonly Dictionary<string, string> CooldownDict = new Dictionary<string, string>
        {
            { Russian, "перезарядка" },
            { English, "cooldown" },
            { Turkish, "bekleme süresi" },
        };

        private static readonly Dictionary<string, string> HitCountDict = new Dictionary<string, string>
        {
            { Russian, "количество ударов" },
            { English, "hit count" },
            { Turkish, "vuruş sayısı" },
        };

        private static readonly Dictionary<string, string> BouncesCountDict = new Dictionary<string, string>
        {
            { Russian, "количество отскоков" },
            { English, "bounces count" },
            { Turkish, "zıplama sayısı" },
        };

        private static readonly Dictionary<string, string> CooldownPerHitDict = new Dictionary<string, string>
        {
            { Russian, "перезарядка за удар" },
            { English, "cooldown per hit" },
            { Turkish, "vuruş başına bekleme" },
        };

        private static readonly Dictionary<string, string> BounceRangeDict = new Dictionary<string, string>
        {
            { Russian, "радиус отскока" },
            { English, "bounce range" },
            { Turkish, "zıplama menzili" },
        };

        private static readonly Dictionary<string, string> RadiusDict = new Dictionary<string, string>
        {
            { Russian, "радиус" },
            { English, "radius" },
            { Turkish, "yarıçap" },
        };

        private static readonly Dictionary<string, string> RotationSpeedDict = new Dictionary<string, string>
        {
            { Russian, "скорость вращения" },
            { English, "rotation speed" },
            { Turkish, "dönüş hızı" },
        };

        private static readonly Dictionary<string, string> IncreaseValueDict = new Dictionary<string, string>
        {
            { Russian, "значение увеличения" },
            { English, "increase value" },
            { Turkish, "artış değeri" },
        };

        private static readonly Dictionary<string, string> CountDict = new Dictionary<string, string>
        {
            { Russian, "количество" },
            { English, "count" },
            { Turkish, "saymak" },
        };

        private static readonly Dictionary<string, string> MultiplierDict = new Dictionary<string, string>
        {
            { Russian, "Множитель к" },
            { English, "Multiplier to" },
            { Turkish, "Çarpan" },
        };

        private static readonly Dictionary<string, string> MaxLevelDict = new Dictionary<string, string>
        {
            { Russian, "Макс. уровень" },
            { English, "Max Level" },
            { Turkish, "Maks Seviye" },
        };

        private static readonly Dictionary<string, string> ShopDict = new Dictionary<string, string>
        {
            { Russian, "Maгазин" },
            { English, "Shop" },
            { Turkish, "Mağaza" },
        };

        private static readonly Dictionary<string, string> LVLDict = new Dictionary<string, string>
        {
            { Russian, "Ур" },
            { English, "LVL" },
            { Turkish, "LVL" },
        };

        private static readonly Dictionary<string, string> YourBestScoreDict = new Dictionary<string, string>
        {
            { Russian, "ВАШ ЛУЧШИЙ СЧЕТ" },
            { English, "YOUR BEST SCORE" },
            { Turkish, "EN İYİ SKORUN" },
        };

        private static readonly Dictionary<string, string> NoBestScoreDict = new Dictionary<string, string>
        {
            { Russian, "НЕТ СЧЕТА" },
            { English, "NO SCORE" },
            { Turkish, "PUAN YOK" },
        };


        public static string YourBestScore => GetText(YourBestScoreDict);
        public static string BurnDuration => GetText(BurnDurationDict);
        public static string FreezeDuration => GetText(FreezeDurationDict);
        public static string BurnDPS => GetText(BurnDPSDict);
        public static string SlowPercent => GetText(SlowPercentDict);

        public static string NoBestScore => GetText(NoBestScoreDict);

        public static string Damage => GetText(DamageDict);

        public static string AttackDelay => GetText(AttackDelayDict);

        public static string AttackRange => GetText(AttackRangeDict);

        public static string FlightDistance => GetText(FlightDistanceDict);

        public static string Cooldown => GetText(CooldownDict);

        public static string HitCount => GetText(HitCountDict);

        public static string BouncesCount => GetText(BouncesCountDict);

        public static string CooldownPerHit => GetText(CooldownPerHitDict);

        public static string BounceRange => GetText(BounceRangeDict);

        public static string Radius => GetText(RadiusDict);

        public static string RotationSpeed => GetText(RotationSpeedDict);

        public static string IncreaseValue => GetText(IncreaseValueDict);

        public static string Count => GetText(CountDict);

        public static string Multiplier => GetText(MultiplierDict);

        public static string Shop => GetText(ShopDict);

        public static string LVL => GetText(LVLDict);

        public static string MaxLevel => GetText(MaxLevelDict);

        private static string CurrentLanguage => YG2.lang;

        public static string GetEntityTypeText(EntityType type) => GetLocalizedText(type, EntityTypeDict);

        public static string GetWindowTypeText(WindowType type) => GetLocalizedText(type, WindowTypeDict);

        public static string GetStatTypeText(StatType type) => GetLocalizedText(type, StatTypeDict);

        private static string GetLocalizedText<T>(T key, Dictionary<T, Dictionary<string, string>> dict)
        {
            if (dict.TryGetValue(key, out var translations))
            {
                if (string.IsNullOrEmpty(CurrentLanguage) == false && translations.TryGetValue(CurrentLanguage, out var localized))
                {
                    return localized;
                }

                if (translations.TryGetValue(English, out var en))
                {
                    return en;
                }
            }

            return key.ToString();
        }

        private static string GetText(Dictionary<string, string> dict)
        {
            if (dict.TryGetValue(CurrentLanguage, out string value))
            {
                return value;
            }

            return dict[English];
        }
    }
}