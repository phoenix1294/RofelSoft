using Newtonsoft.Json;
using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class Item
    {

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("Count")]
        public int Count { get; set; }
        [JsonProperty("tag")]
        public Tag Tags { get; set; }
        public Item()
        {

        }
        public Item(string id, int count, Tag tags)
        {
            Id = id;
            Count = count;
            Tags = tags;
        }
        public string Serialize()
        {
            var item = new StringBuilder();
            item.Append($"{Id}");
            if (Tags != null)
            {
                item.Append("{");
                item.Append($"{Tags.Serialize()}");
                item.Append("}");
            }
            item.Append($" {Count}");
            return item.ToString();
        }
    }
}
