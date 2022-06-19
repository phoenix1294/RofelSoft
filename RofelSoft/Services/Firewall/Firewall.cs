using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace RofelSoft.Services.Firewall
{
    class Firewall : IService
    {
        public List<string> ActualList = new List<string>();

        public string ServiceName { get; set; }

        public ServiceStatus Status { get; set; }

        public Firewall()
        {
            ServiceName = "Firewall";
        }

        private void FirewallThread()
        {
            while (Status == ServiceStatus.Running)
            {
                var users = CraftApi.GetUserList();

                for (int i = 0; i < users.Length; i++)
                {
                    if (users[i] != "")
                    {
                        TrackUser(new User(users[i]));
                        var cusr = RootService.Users.Where((User user) => { return user.Name == users[i]; }).ToList();
                        if (cusr.Count < 1 || !cusr[0].Authorized)
                        {
                            Logger.Warn(users[i] + " Was kicked");
                            RootService.Connector.Command("/kick " + users[i] + $" Авторизуйся на http://{RootService.Settings.WebHost}/");
                        }
                        else
                        {
                            cusr[0].IngameWait = false;
                        }
                    }
                }

                try
                {
                    foreach (string user in ActualList)
                    {
                        if (!users.Contains(user))
                        {
                            var cusr = RootService.Users.Where((User x) => { return x.Name == user; }).ToList();
                            if (!cusr[0].IngameWait)
                            {
                                cusr[0].Authorized = false;
                                Logger.Warn(cusr[0].Name + " Was unauthorized");
                                ActualList.Remove(user);
                            }
                        }
                    }
                }
                catch { }

                Thread.Sleep(500);
            }
        }

        private static void TrackUser(User user)
        {
            if (!Directory.Exists("Records"))
            {
                Directory.CreateDirectory("Records");
            }
            var pos = CraftApi.GetUserPosition(user);
            pos ??= new Vector(0, 0, 0);
            File.AppendAllText("Records/" + user.Name + ".trk", "(" + DateTime.Now + ") " + CraftApi.GetUserDimension(user) + ":" + pos.ToString() + "\r");
        }

        public bool StartService()
        {
            Status = ServiceStatus.Running;
            new Thread(FirewallThread).Start();
            return true;
        }

        public bool StopService()
        {
            Status = ServiceStatus.Stopped;
            return true;
        }
    }
}
