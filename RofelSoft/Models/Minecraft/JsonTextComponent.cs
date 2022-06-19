using Newtonsoft.Json;
using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class JsonTextComponent
    {
        //{ "text":"string",
        //"bold":true, "italic":bool,
        //"strikethrough":bool,
        //"underlined":bool,
        //"obfuscated":bool,
        //"color":"color"
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("bold")]
        public bool Bold { get; set; }
        [JsonProperty("italic")]
        public bool Italic { get; set; }
        [JsonProperty("strikethrough")]
        public bool Strikethrough { get; set; }
        [JsonProperty("underlined")]
        public bool Underlined { get; set; }
        [JsonProperty("obfuscated")]
        public bool Obfuscated { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }

        public string Serialize()
        {
            var str = new StringBuilder();
            str.Append("{");
            bool ftag = true;
            if (Text != "")
            {
                if (!ftag) { str.Append(","); }
                str.Append($"\"text\":\"{Text}\"");
                ftag = false;
            }
            if (Color != null)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"\"color\":\"{Color}\"");
                ftag = false;
            }
            if (Obfuscated)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"\"obfuscated\":\"true\"");
                ftag = false;
            }
            if (Underlined)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"\"underlined\":\"true\"");
                ftag = false;
            }
            if (Strikethrough)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"\"strikethrough\":\"true\"");
                ftag = false;
            }
            if (Bold)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"\"bold\":\"true\"");
                ftag = false;
            }
            if (!Italic)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"\"italic\":\"false\"");
            }
            str.Append("}");
            return str.ToString();
        }
    }
}
