using System.Text;

namespace RofelSoft.Models.Minecraft
{
    // Half-fully NBT Tag representation
    class Tag
    {
        public Enchantment[] Enchantments { get; set; } // Serialized
        public Enchantment[] StoredEnchantments { get; set; } // Serialized
        public Display Display { get; set; } // Serialized
        public int Damage { get; set; } // Serialized
        public int Unbreakable { get; set; }
        public BlockEntityTag BlockEntityTag { get; set; } // Serialized
        public int HideFlags { get; set; }
        public int RepairCost { get; set; }
        public AttributeModifier[] AttributeModifiers { get; set; }
        public CustomPotionEffect[] CustomPotionEffects { get; set; }
        public string Potion { get; set; }
        public int CustomPotionColor { get; set; }
        public string Pages { get; set; }
        public string Author { get; set; }
        public Explosion Explosion { get; set; }
        public Tag(Display display = null, Enchantment[] enchantments = null, Enchantment[] stored = null)
        {
            Display = display;
            Enchantments = enchantments;
            StoredEnchantments = stored;
        }

        public string Serialize()
        {
            var str = new StringBuilder();
            bool ftag = true;
            if (Enchantments != null && Enchantments.Length > 0)
            {
                str.Append("Enchantments:[");
                for (int z = 0; z < Enchantments.Length; z++)
                {
                    str.Append(Enchantments[z].Serialize());
                    if (z < Enchantments.Length - 1) { str.Append(","); };
                }
                str.Append("]");
                ftag = false;
            }
            if (StoredEnchantments != null && StoredEnchantments.Length > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append("StoredEnchantments:[");
                for (int z = 0; z < StoredEnchantments.Length; z++)
                {
                    str.Append(StoredEnchantments[z].Serialize());
                    if (z < StoredEnchantments.Length - 1) { str.Append(","); };
                }
                str.Append("]");
                ftag = false;
            }
            if (Damage > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Damage:{Damage}");
                ftag = false;
            }
            if (Display != null)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"display:{Display.Serialize()}");
                ftag = false;
            }
            if (BlockEntityTag != null)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"BlockEntityTag:{BlockEntityTag.Serialize()}");
                ftag = false;
            }
            return str.ToString();
        }
    }
    class AttributeModifier
    {
        public string AttributeName { get; set; }
        public string Slot { get; set; }
    }
}
