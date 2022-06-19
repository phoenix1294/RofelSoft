using Newtonsoft.Json;
using System.Text;
namespace RofelSoft.Models.Minecraft
{
    class Display
    {
        [JsonProperty(PropertyName = "Lore")]
        public JsonTextComponent[] Lore { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public JsonTextComponent Name { get; set; }

        public string Serialize()
        {
            var str = new StringBuilder();
            bool ftag = true;
            str.Append("{");

            if (Lore != null && Lore.Length > 0)
            {
                str.Append("Lore:[");
                for (int z = 0; z < Lore.Length; z++)
                {
                    str.Append($"'{Lore[z].Serialize()}'");
                    if (z < Lore.Length - 1) { str.Append(","); };
                }
                str.Append("]");
                ftag = false;
            }
            if (Name != null)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Name:'{Name.Serialize()}'");
            }
            str.Append("}");
            return str.ToString();
        }
    }
}
