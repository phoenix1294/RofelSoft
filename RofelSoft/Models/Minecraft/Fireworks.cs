using System.Text;

namespace RofelSoft.Models.Minecraft
{
    class Fireworks
    {
        public Explosion[] Explosions { get; set; }
        // Fireworks:{Explosions:[<Explosion>]}
        // Single Tag
        public string Serialize()
        {
            var str = new StringBuilder();
            str.Append("Fireworks:{");
            if (Explosions != null && Explosions.Length > 0)
            {
                str.Append("Explosions:[");
                for (int z = 0; z < Explosions.Length; z++)
                {
                    str.Append(Explosions[z].Serialize());
                    if (z < Explosions.Length - 1) { str.Append(','); };
                }
                str.Append("]");
            }
            str.Append("}");
            return str.ToString();
        }
    } // Serializable, Tested
}
