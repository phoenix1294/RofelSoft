using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Utils;
using System;
using System.Linq;

namespace RofelSoft.Services.Market.Transactions
{
    class BuyTransaction : ITransaction
    {
        public string State { get; set; }
        public double Sum { get; set; }
        public User Agent { get; set; }
        private Product Prod;
        private int Count;
        private Random Rand;

        public void LoadData(string[] data, User user)
        {
            Rand = new Random();
            Agent = user;
            Count = int.Parse(data[1]);
            Prod = RootService.Goods.Where(good => good.Id == int.Parse(data[0])).First();
        }

        public double CountTransactionPrice()
        {
            Sum = Prod.ActualPrice;
            if (!Prod.StaticPrice)
            {
                Sum = Prod.ActualPrice + Count / (RootService.PriceFactor / Prod.BeginPrice);
                //RootService.PriceReduct(Prod);
            }
            Sum = Math.Round(Sum, 2);
            return Sum * Count;
        }

        public bool Operation()
        {
            if (Prod.FirstSold)
            {
                throw new Exception("В корпорации нет образцов для продажи, продайте нам хотя бы один...");
            }
            if (Sum * Count <= Agent.Money)
            {
                if (Count <= 0)
                {
                    State = $"Нельзя купить {Count} предметов";
                    return false;
                }
                else
                {
                    CraftApi.GiveItem(Agent, Prod, Count);
                    CraftApi.PlaySound(Agent, "minecraft:block.amethyst_cluster.hit");
                    State = $"Приобретено {Count} {Prod.ItemName} за {Sum * Count} пикселей!";
                    return true;
                }

            }
            else
            {
                State = $"Недостаточно пикселей";
                return false;
            }
        }

        public void CountMoney()
        {
            Agent.AddMoney(-Sum * Count);
            if (!Prod.StaticPrice)
            {
                Prod.ActualPrice = Math.Round(Sum, 2);
            }
        }
    }
}
