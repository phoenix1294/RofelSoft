using RofelSoft.Interfaces;
using RofelSoft.Models;

namespace RofelSoft.Services.Market.Transactions
{
    class XpTransaction : ITransaction
    {
        public User Agent { get; set; }
        public double Sum { get; set; }
        public string State { get; set; }

        public double Multiplier = 10;

        public void CountMoney()
        {
            Agent.AddMoney(-Sum * Multiplier);
        }

        public double CountTransactionPrice()
        {
            return Sum * Multiplier;
        }

        public void LoadData(string[] data, User user)
        {
            Sum = int.Parse(data[0]);
            Agent = user;
        }

        public bool Operation()
        {
            if (Sum * Multiplier <= Agent.Money)
            {
                if (Sum <= 0)
                {
                    State = $"Нельзя купить {Sum} опыта";
                    return false;
                }
                else
                {
                    RootService.Connector.Command($"/execute at {Agent.Name} as {Agent.Name} run summon minecraft:experience_orb ~ ~ ~ {{Value:{Sum}s}}");
                    State = $"Успешно выдано {Sum} опыта";
                    return true;
                }
            }
            else
            {
                State = "Недостаточно средств";
                return false;
            }
        }
    }
}
