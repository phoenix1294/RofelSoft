using Newtonsoft.Json;
using RofelSoft.Models.Minecraft;
using System;

namespace RofelSoft.Converters
{
    class TextComponentParser : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            try
            {
                var ret = JsonConvert.DeserializeObject<JsonTextComponent>(value);
                ret.Italic = !ret.Italic;
                return ret;
            }
            catch
            {
                return new JsonTextComponent()
                {
                    Text = value,
                    Italic = false
                };
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonTextComponent);
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }
    }
}
