using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class BlockEntityTag
    {
        public string Lock { get; set; }
        public string Command { get; set; }
        public ItemStack[] Items { get; set; }
        public string Serialize()
        {
            var str = new StringBuilder();
            bool ftag = true;
            str.Append("{");
            if (Items != null && Items.Length > 0)
            {
                str.Append("Items:[");
                for (int z = 0; z < Items.Length; z++)
                {
                    str.Append(Items[z].Serialize());
                    if (z < Items.Length - 1) { str.Append(','); };
                }
                str.Append("]");
                ftag = false;
            }
            if (Lock != null)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Lock:\"{Lock}\"");
                ftag = false;
            }
            if (Command != null)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Command:\"{Command}\"");
            }
            str.Append("}");
            return str.ToString();
        }
    }
}
