using System.Text;
namespace RofelSoft.Models.Minecraft
{
    class ItemStack : Item
    {
        public int Slot { get; set; }
        public ItemStack()
        {

        }
        public ItemStack(string id, int count)
        {
            Id = id;
            Count = count;
        }
        public ItemStack(string id, int count, Tag tags)
        {
            Id = id;
            Count = count;
            Tags = tags;
        }
        public new string Serialize()
        {
            var item = new StringBuilder();
            item.Append("{");
            item.Append($"id:\"{Id}\",");
            item.Append($"Count:{Count}b,");
            item.Append($"Slot:{Slot}b");
            if (Tags != null)
            {
                item.Append(", ");
                item.Append("tag:{");
                item.Append($"{Tags.Serialize()}");
                item.Append("}");
            }
            item.Append("}");
            return item.ToString();
        }

    }
}
