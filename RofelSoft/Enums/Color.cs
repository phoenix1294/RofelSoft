namespace RofelSoft.Enums
{
    /// <summary>1.18.1 Json text color list</summary>
    class Color
    {
        public static readonly Color Black = "black"; //+
        public static readonly Color DarkBlue = "dark_blue"; //+
        public static readonly Color DarkGreen = "dark_green"; //+
        public static readonly Color DarkAqua = "dark_aqua"; //+
        public static readonly Color DarkPurple = "dark_purple"; //+
        public static readonly Color Gold = "gold"; //+
        public static readonly Color Gray = "gray"; //+
        public static readonly Color DarkGray = "dark_gray"; //+
        public static readonly Color Blue = "blue"; //+
        public static readonly Color Green = "green"; //+
        public static readonly Color Aqua = "aqua"; //+
        public static readonly Color Red = "red"; //+
        public static readonly Color LightPurple = "light_purple"; //+
        public static readonly Color Yellow = "yellow"; //+
        public static readonly Color White = "white"; //+
        private string value;
        private Color(string e)
        {
            value = e;
        }
        public static implicit operator Color(string v)
        {
            return new Color(v);
        }

        public static implicit operator string(Color v)
        {
            return v.value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}