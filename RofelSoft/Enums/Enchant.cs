namespace RofelSoft.Enums
{
    /// <summary>1.18.1 Enchantment list</summary>
    internal class Enchant
    {
        /// <summary>Защита</summary>
        public static readonly Enchant Protection = "protection"; //+
        /// <summary>Огнеупорность</summary>
        public static readonly Enchant FireProtection = "fire_protection"; //+
        /// <summary>Невесомость</summary>
        public static readonly Enchant FeatherFalling = "feather_falling"; //+
        /// <summary>Взрывоустойчивость</summary>
        public static readonly Enchant BlastProtection = "blast_protection"; //+
        /// <summary>Защита от снарядов</summary>
        public static readonly Enchant ProjectileProtection = "projectile_protection";  //+
        /// <summary>Подводное дыхание</summary>
        public static readonly Enchant Respiration = "respiration"; //+
        /// <summary>Подводник</summary>
        public static readonly Enchant AquaAffinity = "aqua_affinity"; //+
        /// <summary>Шипы</summary>
        public static readonly Enchant Thorns = "thorns"; //+
        /// <summary>Глубинный шаг</summary>
        public static readonly Enchant DepthStrider = "depth_strider"; //-
        /// <summary>Ледоход</summary>
        public static readonly Enchant FrostWalker = "frost_walker"; //+
        /// <summary>Проклятие несъёмности</summary>
        public static readonly Enchant CurseOfBinding = "curse_of_binding"; //+
        /// <summary>Острота</summary>
        public static readonly Enchant Sharpness = "sharpness"; //+
        /// <summary>Небесная кара</summary>
        public static readonly Enchant Smite = "smite"; //+
        /// <summary>Бич членистоногих</summary>
        public static readonly Enchant BaneOfArthropods = "bane_of_athropods"; //+
        /// <summary>Откидывание</summary>
        public static readonly Enchant Knockback = "knockback"; //+
        /// <summary>Заговор огня</summary>
        public static readonly Enchant FireAspect = "fire_aspect"; //+
        /// <summary>Добыча</summary>
        public static readonly Enchant Looting = "looting"; //+
        /// <summary>Разящий клинок</summary>
        public static readonly Enchant SweepengEdge = "sweep"; //+
        /// <summary>Эффективность</summary>
        public static readonly Enchant Efficiency = "efficiency"; //+
        /// <summary>Шелковое касание</summary>
        public static readonly Enchant SilkTouch = "silk_touch"; //+
        /// <summary>Прочность</summary>
        public static readonly Enchant Unbreaking = "unbreaking"; //+
        /// <summary>Удача</summary>
        public static readonly Enchant Fortune = "fortune"; //+
        /// <summary>Сила</summary>
        public static readonly Enchant Power = "power"; //+
        /// <summary>Отбрасывание</summary>
        public static readonly Enchant Punch = "punch"; //+
        /// <summary>Горящая стрела</summary>
        public static readonly Enchant Flame = "flame"; //+
        /// <summary>Бесконечность</summary>
        public static readonly Enchant Infinity = "infinity"; //+
        /// <summary>Морская удача</summary>
        public static readonly Enchant LuckOfSea = "luck_of_sea"; //+
        /// <summary>Приманка</summary>
        public static readonly Enchant Lure = "lure"; //+
        /// <summary>Починка</summary>
        public static readonly Enchant Mending = "mending"; //+
        /// <summary>Проклятие утраты</summary>
        public static readonly Enchant CurseOfVanishing = "curse_of_vanishing"; //+
        /// <summary>Пронзатель</summary>
        public static readonly Enchant Impaling = "impaling"; //+
        /// <summary>Громовержец</summary>
        public static readonly Enchant Channeling = "channeling"; //+
        /// <summary>Верность</summary>
        public static readonly Enchant Loyality = "loyality"; //+
        /// <summary>Тягун</summary>
        public static readonly Enchant Riptide = "riptide"; //+
        /// <summary>Тройной выстрел</summary>
        public static readonly Enchant Multishot = "multishot"; //+
        /// <summary>Скорость души</summary>
        public static readonly Enchant SoulSpeed = "soul_speed"; //+
        /// <summary>Быстрая перезарядка</summary>
        public static readonly Enchant QuickCharge = "quick_charge"; //+
        /// <summary>Пустое зачарование</summary>
        public static readonly Enchant Dummy = "dummy"; //+
        private readonly string value;
        private Enchant(string e)
        {
            value = e;
        }
        public static implicit operator Enchant(string v)
        {
            return new Enchant(v);
        }
        public static implicit operator string(Enchant v)
        {
            return v.value;
        }

        public static bool operator ==(Enchant v, Enchant z)
        {
            return v.value == z.value;
        }

        public static bool operator !=(Enchant v, Enchant z)
        {
            return !(v.value == z.value);
        }

        public override string ToString()
        {
            return value;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}