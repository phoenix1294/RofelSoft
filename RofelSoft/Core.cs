using RofelSoft.Utils;
using System;

namespace RofelSoft
{
    class Core
    {
        static RootService root = new RootService();
        static void Main()
        {
            Console.Title = "RofelSoft Microservices Platform Console";
            Console.CancelKeyPress += Abort;
            if (!root.StartService())
            {
                Logger.Error("Unable to start root service");
            }
            while (true)
            {
                Console.ReadLine(); // command proceeder
            }

        }

        private static void Abort(object sender, ConsoleCancelEventArgs e)
        {
            root.StopService();
            Environment.Exit(0);
        }
    }
}
