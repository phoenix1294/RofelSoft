using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Utils;
using System;

namespace RofelSoft.Services.Market.Transactions
{
    class TpTransaction : ITransaction
    {

        public User Agent { get; set; }
        private Vector PlayerPosition { get; set; }
        public double Sum { get; set; }
        public string State { get; set; }

        private int _x = 0;
        private int _y = 0;
        private int _z = 0;

        public void CountMoney()
        {
            Agent.AddMoney(-Sum);
        }

        public double CountTransactionPrice()
        {
            var dim = CraftApi.GetUserDimension(Agent);
            int multiplex = (dim == "minecraft:overworld") ? 2 : (dim == "minecraft:the_nether") ? 8 : (dim == "minecraft:the_end") ? 1 : 10;
            Sum = multiplex * (double)Math.Sqrt(
                Math.Pow(Math.Abs(_x - PlayerPosition.X), 2) +
                Math.Pow(Math.Abs(_y - PlayerPosition.Y), 2) +
                Math.Pow(Math.Abs(_z - PlayerPosition.Z), 2));
            Sum = Math.Round(Sum, 2);
            return Sum;
        }

        public void LoadData(string[] data, User user)
        {
            Agent = user;
            PlayerPosition = CraftApi.GetUserPosition(Agent);
            _x = int.Parse(data[0]);
            _y = int.Parse(data[1]);
            _z = int.Parse(data[2]);
        }

        public bool Operation()
        {
            if (Sum <= Agent.Money)
            {
                if (Math.Abs(_x) < 29999983 & Math.Abs(_z) < 29999983 & _y > -65 & _y < 256)
                {
                    RootService.Connector.Command($"/execute as {Agent.Name} at {Agent.Name} run tp {_x} {_y} {_z}");
                    CraftApi.PlaySound(Agent, "minecraft:entity.enderman.teleport");
                    State = $"Вы были телепортированы в точку [{_x}, {_y}, {_z}] за {Sum} пикселей";
                    return true;
                }
                else
                {
                    State = $"Недопустимые координаты";
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
