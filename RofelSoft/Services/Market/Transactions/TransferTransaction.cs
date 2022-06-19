using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Utils;
using System;
using System.Linq;

namespace RofelSoft.Services.Market.Transactions
{
    class TransferTransaction : ITransaction
    {
        public string State { get; set; }
        public double Sum { get; set; }
        public User Agent { get; set; }
        private User Receiver { get; set; }

        public void CountMoney()
        {
            Agent.AddMoney(-Sum);
            Receiver.AddMoney(Sum);
        }

        public double CountTransactionPrice()
        {
            return Sum;
        }

        public void LoadData(string[] data, User user)
        {
            Agent = user;
            Sum = Math.Round(double.Parse(data[1]), 2);
            Receiver = RootService.Users.Where(p => p.Name == data[0]).First();
        }

        public bool Operation()
        {
            if (Sum <= Agent.Money)
            {
                if (Sum > 0)
                {
                    CraftApi.Say($"{RootService.MarketService.ServiceName} {Agent.Name} перевёл вам {Sum} пикселей", Receiver.Name, Color.Aqua);
                    CraftApi.PlaySound(Receiver, "minecraft:block.amethyst_cluster.hit");
                    State = $"Перевод выполнен успешно";
                    return true;
                }
                else
                {
                    State = $"Нулевые и отрицательные значения запрещены";
                    return false;
                }
            }
            else
            {
                State = $"Недостаточно пикселей";
                return false;
            }
        }
    }
}
