using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RofelSoft.Services.Regulation
{
    class Regulation : IService
    {
        private Random Rnd = new Random();

        public string ServiceName { get; set; }

        public ServiceStatus Status { get; set; }

        public Regulation()
        {
            ServiceName = "Regulation";
        }

        public bool StartService()
        {
            new Thread(RegulationThread).Start();
            Status = ServiceStatus.Running;
            return true;
        }

        public bool StopService()
        {
            Status = ServiceStatus.Stopped;
            return true;
        }

        private void RegulationThread()
        {
            while(Status == ServiceStatus.Running)
            {
                Tick();
                Thread.Sleep(RootService.TickInterval);
            }
        }

        private void Tick()
        {
            while (!RootService.CanModify) { Thread.Sleep(50); }
            var count = RootService.Goods.Count;
            for (int z = 0; z < count; z++)
            {
                if (!RootService.Goods[z].StaticPrice)
                {
                    var e = Rnd.Next(0, 10) * 0.0001d;   // 0 1 2 3 4 (5) 6 7 8 9
                    if (RootService.Goods[z].ActualPrice > RootService.Goods[z].BeginPrice * 2)
                    {
                        RootService.Goods[z].ActualPrice /= 1.0005d - e;
                    }
                    else
                    {
                        RootService.Goods[z].ActualPrice *= 1.0005d - e;
                    }
                    if (RootService.Goods[z].PriceHistory == null)
                    {
                        RootService.Goods[z].PriceHistory = new List<double>();
                    }
                    if (RootService.Goods[z].PriceHistory.Count > RootService.HistoryLength)
                    {
                        RootService.Goods[z].PriceHistory.RemoveAt(0);
                    }
                    RootService.Goods[z].ActualPrice = Math.Round(RootService.Goods[z].ActualPrice, 2);
                    RootService.Goods[z].PriceHistory.Add(RootService.Goods[z].ActualPrice);
                    var i = RootService.Goods[z].PriceHistory.Count;
                    if (i > 2)
                    {
                        var percentage = (RootService.Goods[z].PriceHistory[i - 1] - RootService.Goods[z].PriceHistory[i - 2]) / RootService.Goods[z].PriceHistory[i - 2] * 100;
                        if (percentage > 10)
                        {
                            CraftApi.Broadcast($"{RootService.MarketService.ServiceName} Корпорация предлагает продавать {RootService.Goods[z].ItemName}! Он подорожал на {Math.Round(Math.Abs(percentage), 2)}%!", Color.LightPurple);
                        }
                        else
                        {
                            if (percentage < -10)
                            {
                                CraftApi.Broadcast($"{RootService.MarketService.ServiceName} Корпорация предлагает покупать {RootService.Goods[z].ItemName}! Он подешевел на {Math.Round(Math.Abs(percentage), 2)}%!", Color.LightPurple);
                            }
                        }
                    }
                }
                else
                {
                    RootService.Goods[z].PriceHistory = null;
                }
            }
        }
    }
}
