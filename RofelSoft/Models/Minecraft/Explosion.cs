using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class Explosion
    {
        public int[] Colors { get; set; }
        public int[] FadeColors { get; set; }
        public byte Flicker { get; set; }
        public byte Trail { get; set; }
        public byte Type { get; set; }

        // Colors - optional
        // FadeColors - optional
        // Flicker - optional
        // Trail - optional
        // Type - optional
        // Empty - {} - is valid
        // Full - {Colors:[I;11743532],FadeColors:[I;15790320],Flicker:1b,Trail:0b,Type:4b}
        public string Serialize()
        {
            var str = new StringBuilder();
            bool ftag = true;
            str.Append("{");
            if (Colors != null && Colors.Length > 0)
            {
                str.Append("Colors:[I;");
                for (int z = 0; z < Colors.Length; z++)
                {
                    str.Append(Colors[z]);
                    if (z < Colors.Length - 1) { str.Append(','); };
                }
                str.Append("]");
                ftag = false;
            }
            if (FadeColors != null && FadeColors.Length > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append("FadeColors:[I;");
                for (int z = 0; z < FadeColors.Length; z++)
                {
                    str.Append(FadeColors[z]);
                    if (z < FadeColors.Length - 1) { str.Append(','); };
                }
                str.Append("]");
                ftag = false;
            }
            if (Flicker > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Flicker:{Flicker}b");
                ftag = false;
            }
            if (Trail > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Trail:{Trail}b");
                ftag = false;
            }
            if (Type > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Type:{Type}b");
            }
            str.Append("}");
            return str.ToString();
        }
    } // Serializable, Tested
}
