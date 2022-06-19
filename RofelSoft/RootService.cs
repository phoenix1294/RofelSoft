using Newtonsoft.Json;
using RofelSoft.Converters;
using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Models.Minecraft;
using RofelSoft.Services.Admin;
using RofelSoft.Services.Firewall;
using RofelSoft.Services.Market;
using RofelSoft.Services.News;
using RofelSoft.Services.Regulation;
using RofelSoft.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RofelSoft
{
    class RootService : IService
    {
        public static Market MarketService = new Market();
        public static News NewsService = new News();
        public static Admin AdminService = new Admin();
        public static Regulation RegulationService = new Regulation();
        public static Firewall FirewallService = new Firewall();

        public static Settings Settings;
        public static RconQueue Connector;
        public static JsonConverter[] Converters;
        public static List<User> Users;

        public string ServiceName { get; set; }
        public ServiceStatus Status { get; set; }
        public static bool CanModify = true;
        public static int HistoryLength { get; internal set; }

        public static double ReductionBorder = 0.05;
        public static int TickInterval = 5000;
        public static int PriceFactor = 1200;
        public static List<Product> Goods;
        public static List<ItemStack> Loots;

        public RootService()
        {
            InitializeConverters();
            DataHelper.ReadAll();

            Status = ServiceStatus.Stopped;
            ServiceName = "RofelSoft Root";
            Connector = new RconQueue(8, new Uri($"tcp://{Settings.RconHost}"));
        }

        private void InitializeConverters()
        {
            Converters = new JsonConverter[]
            {
                new TextComponentParser(),
                new EnchantParser()
            };
        }

        public static string GetUserList(User current)
        {
            string userlist = "[";
            foreach (User user in Users)
            {
                if (user.Name != current.Name)
                {
                    userlist += $"\"{user.Name}\",";
                }
            }
            userlist = userlist[0..^1];
            userlist += "]";
            return userlist;
        }

        private void InviteService(IService service)
        {
            Task.Run(() => { service.StartService(); });
            Logger.Success("\'" + service.ServiceName + "\' service  was started");
        }

        private void UnloadService(IService service)
        {
            service.StopService();
            Logger.Success("\'" + service.ServiceName + "\' service was stopped");
        }

        public bool StartService()
        {
            InviteService(MarketService);
            InviteService(NewsService);
            InviteService(AdminService);
            InviteService(RegulationService);
            InviteService(FirewallService);
            Status = ServiceStatus.Running;
            return true;
        }

        public bool StopService()
        {
            UnloadService(MarketService);
            UnloadService(NewsService);
            UnloadService(AdminService);
            UnloadService(RegulationService);
            UnloadService(FirewallService);
            DataHelper.WriteAll();
            Status = ServiceStatus.Stopped;
            return true;
        }
    }
}
