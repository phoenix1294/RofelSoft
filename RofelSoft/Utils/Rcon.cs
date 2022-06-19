using RofelSoft.Enums;
using RofelSoft.Models;
using System;
using System.Net.Sockets;
using System.Text;

namespace RofelSoft.Utils
{
    class Rcon
    {
        private readonly Uri Url;
        private Socket Session;
        public bool Busy { get; private set; }
        private bool Logged = false;

        public Rcon(Uri hostname)
        {
            Url = hostname;
        }

        public void Login()
        {
            Session = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Session.Connect(Url.Host, Url.Port);
            var packet = new Packet(RootService.Settings.RconPassword, RequestType.Login);
            Session.Send(packet.Content);
            byte[] buffer = new byte[4110];
            Session.Receive(buffer);
            if (Send("/list").Contains("Unknown request 0"))
            {
                Logged = false;
            }
            else
            {
                Logged = true;
            }
        }

        public string Command(string command)
        {
            if (Logged)
            {
                try
                {
                    return Send(command);
                }
                catch
                {
                    AutoRelogin();
                    Logger.Warn("Trying execute again");
                    return Command(command);
                }
            }
            else
            {
                throw new Exception("Not connected to RCON");
            }
        }

        private void AutoRelogin()
        {
            Session.Close();
            Login();
            Busy = false;
        }

        private string Send(string command)
        {
            Busy = true;
            Packet packet = new Packet(command, RequestType.Command);
            Session.Send(packet.Content);
            byte[] buf = new byte[4];
            Session.Receive(buf);
            int toReceive = BitConverter.ToInt32(buf);
            if (toReceive > 2097151)
            {
                throw new Exception("Too big response, something broken");
            }
            else
            {
                buf = new byte[toReceive];
                if (Session.Receive(buf) > 0)
                {
                    Busy = false;
                    return Encoding.UTF8.GetString(buf, 8, toReceive - 10);
                }
                else
                {
                    Busy = false;
                    return "";
                }
            }
        }
    }
}
