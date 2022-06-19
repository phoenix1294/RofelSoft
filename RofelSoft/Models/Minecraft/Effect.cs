using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class Effect
    {
        public byte EffectId { get; set; } = 255;
        public int EffectDuration { get; set; }
        public string Serialize()
        {
            var str = new StringBuilder();
            bool ftag = true;
            str.Append("{");
            if (EffectId < 255)
            {
                str.Append($"EffectId:{EffectId}b");
                ftag = false;
            }
            if (EffectDuration > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"EffectDuration:{EffectDuration}");
            }
            str.Append("}");
            return str.ToString();
        }
    }
}
