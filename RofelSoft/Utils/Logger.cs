using System;
using System.IO;

namespace RofelSoft.Utils
{
    class Logger
    {
        public static readonly ConsoleColor defColor = ConsoleColor.White;

        public static void Debug(object towrite)
        {
            if (RootService.Settings.Debug)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Print(towrite);
            }
        }

        public static void Success(object towrite)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Print(towrite);
        }

        public static void Warn(object towrite)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Print(towrite);
        }

        public static void Log(object towrite)
        {
            Console.ForegroundColor = defColor;
            Print(towrite);
        }

        public static void Error(object towrite)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Print(towrite);
        }

        private static void Print(object towrite)
        {
            var txt = $"[LOGGER] " + towrite;
            File.AppendAllText("latest.log", txt + '\n');
            Console.WriteLine(txt);
        }
    }
}
