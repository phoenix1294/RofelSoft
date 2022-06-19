using System;

namespace RofelSoft.Utils
{
    class RconQueue
    {
        private readonly Rcon[] Threads;
        private readonly int ThreadCount;

        public RconQueue(int threads, Uri uri)
        {
            ThreadCount = threads;
            Threads = new Rcon[threads];
            for (int k = 0; k < threads; k++)
            {
                Threads[k] = new Rcon(uri);
                Threads[k].Login();
            }
        }

        private int CountBusy()
        {
            var ret = 0;
            for (int k = 0; k < ThreadCount; k++)
            {
                if (Threads[k].Busy)
                {
                    ret++;
                }
            }
            return ret;
        }

        public string Command(string command)
        {
            for (int k = 0; k < ThreadCount; k++)
            {
                if (!Threads[k].Busy)
                {
                    return Threads[k].Command(command);
                }
            }
            Logger.Warn($"Queue is overflow? {CountBusy()}/{ThreadCount} busy");
            Logger.Warn("Waiting for free thread...");
            return Command(command);
        }
    }
}
