using Newtonsoft.Json;
using RofelSoft.Enums;
using System;

namespace RofelSoft.Converters
{
    class EnchantParser : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (Enchant)reader.Value.ToString();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Enchant);
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }
    }
}
