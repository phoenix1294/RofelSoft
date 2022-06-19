using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Utils;
using System;
using System.Linq;

namespace RofelSoft.Services.Market.Transactions
{
    class SellTransaction : ITransaction
    {
        public double Sum { get; set; }
        public User Agent { get; set; }
        public string State { get; set; }

        private Product Prod;
        private int Count;

        public void LoadData(string[] data, User user)
        {
            Agent = user;
            Prod = RootService.Goods.Where(good => good.Id == int.Parse(data[0])).First();
            Count = int.Parse(data[1]);
        }

        public double CountTransactionPrice()
        {
            Sum = Prod.ActualPrice;
            if (!Prod.StaticPrice)
            {
                Sum = Prod.ActualPrice - Count / (RootService.PriceFactor / Prod.BeginPrice);
                if (Sum < 0)
                {
                    Sum = Prod.ActualPrice;
                }
            }
            Sum = Math.Round(Sum, 2);
            return Sum * Count;
        }

        public bool Operation()
        {
            if (CraftApi.GetItemCount(Agent, Prod) >= Count)
            {
                if (Count < 1)
                {
                    State = $"Нельзя продать {Count} предметов";
                    return false;
                }
                else
                {
                    Prod.FirstSold = false;
                    CraftApi.TakeItem(Agent, Prod, Count);
                    CraftApi.PlaySound(Agent, "minecraft:block.amethyst_cluster.hit");
                    State = $"Продано {Count} {Prod.ItemName} за {Sum * Count} пикселей";
                    return true;
                }
            }
            else
            {
                State = $"Недостаточно предметов";
                return false;
            }
        }

        public void CountMoney()
        {
            Agent.AddMoney(Sum * Count);
            if (!Prod.StaticPrice)
            {
                Prod.ActualPrice = Math.Round(Sum, 2);
                //RootService.PriceReduct(Prod);
            }
        }
    }
}
