using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RofelSoft.Services.Chunky
{
    class Chunky : IService
    {
        public string ServiceName => throw new NotImplementedException();

        public ServiceStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void PickAnchor()
        {
            byte[] pack = new byte[] { 0x16, 0x00, 0xF5, 0x05, 0x0F, 0x53, 0x75, 0x70, 0x65, 0x72, 0x44, 0x65, 0x61, 0x64, 0x2E, 0x44,
                                       0x6C, 0x69, 0x6E, 0x6B, 0x09, 0x6A, 0x02, 0x0D, 0x00, 0x0B, 0x43, 0x68, 0x75, 0x6E, 0x6B, 0x41,
                                       0x6E, 0x63, 0x68, 0x6F, 0x72 };
            Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            client.Connect(RootService.Settings.RconHost, 2410);
            client.Send(pack);
            var buf = new byte[4];
            client.Receive(buf);
        }

        private void Save(int r, int x, int z)
        {
            List<byte> tmp = new List<byte>();
            tmp.AddRange(BitConverter.GetBytes(r));
            tmp.AddRange(BitConverter.GetBytes(x));
            tmp.AddRange(BitConverter.GetBytes(z));
            File.WriteAllBytes("last.dat", tmp.ToArray());
        }

        private bool CheckToGen()
        {
            var users = CraftApi.GetUserList();
            if (users[0].Length == 0)
            {
                return true;
            }
            else
            {
                if (users.Length == 1)
                {
                    return users[0] == "ChunkAnchor";
                }
                else
                {
                    return false;
                }
            }
        }

        public void Initialize()
        {
            int lastr = 0;
            int lastx = 0;
            int lastz = 0;
            bool launched = true;
            if (File.Exists("last.dat"))
            {
                var e = File.ReadAllBytes("last.dat");
                lastr = BitConverter.ToInt32(e, 0);
                lastx = BitConverter.ToInt32(e, 4);
                lastz = BitConverter.ToInt32(e, 8);
            }

            for (int r = lastr; r < 30000; r += 100)
            {
                for (int x = -r; x < r; x += 100)
                {
                    if (launched) { x = lastx; }
                    for (int z = -r; z < r; z += 100)
                    {
                        if (launched) { z = lastz; launched = false; }

                        if (x == -r || x == r - 100)
                        {
                            while (!CheckToGen()) { Thread.Sleep(1000); }
                            PickAnchor();
                            RootService.Connector.Command($"/tp ChunkAnchor {x} 90 {z}");
                            Thread.Sleep(5000);
                            Console.WriteLine($"[{x};{z}] Generated");
                            Save(r, x, z);
                        }
                        else
                        {
                            if (z == -r || z == r - 100)
                            {
                                while (!CheckToGen()) { Thread.Sleep(1000); }
                                PickAnchor();
                                RootService.Connector.Command($"/tp ChunkAnchor {x} 90 {z}");
                                Thread.Sleep(5000);
                                Console.WriteLine($"[{x};{z}] Generated");
                                Save(r, x, z);
                            }
                        }
                    }
                }
            }

            Console.ReadKey(true);
        }

        public void Tick()
        {

        }

        public bool StartService()
        {
            return true;
        }

        public bool StopService()
        {
            return true;
        }
    }
}
