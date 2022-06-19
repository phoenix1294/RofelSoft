using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class CustomPotionEffect
    {
        //	{CustomPotionEffects:[{Id:#,Amplifier:#,Duration:#}]}
        public byte Id { get; set; } = 255;
        public byte Amplifier { get; set; }
        public int Duration { get; set; }
        public bool Ambient { get; set; }
        public bool ShowParticles { get; set; }
        public bool ShowIcon { get; set; }
        public string Serialize()
        {
            var str = new StringBuilder();
            bool ftag = true;
            str.Append("{");
            if (Id < 255)
            {
                str.Append($"Id:{Id}b");
                ftag = false;
            }
            if (Amplifier > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Amplifier:{Amplifier}b");
                ftag = false;
            }
            if (Duration > 0)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Duration:{Duration}");
            }
            if (Ambient)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"Ambient:{(Ambient ? 1 : 0)}");
            }
            if (ShowParticles)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"ShowParticles:{(ShowParticles ? 1 : 0)}");
            }
            if (ShowIcon)
            {
                if (!ftag) { str.Append(","); }
                str.Append($"ShowParticles:{(ShowIcon ? 1 : 0)}");
            }
            str.Append("}");
            return str.ToString();
        }
    } // Serializable, Tested
}
