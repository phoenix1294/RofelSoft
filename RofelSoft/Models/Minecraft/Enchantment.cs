using Newtonsoft.Json;
using RofelSoft.Enums;
using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class Enchantment
    {
        [JsonProperty("id")]
        public Enchant Id { get; set; }
        [JsonProperty("lvl")]
        public int Level { get; set; }

        public string Serialize()
        {
            var str = new StringBuilder();
            str.Append("{");
            str.Append($"id: \"{(Id.ToString().Contains("minecraft:") ? Id.ToString() : "minecraft:" + Id.ToString())}\",lvl:{Level}s");
            str.Append("}");
            return str.ToString();
        }
    }
}
