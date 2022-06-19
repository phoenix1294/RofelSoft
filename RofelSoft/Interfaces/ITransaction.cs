using RofelSoft.Models;

namespace RofelSoft.Interfaces
{
    interface ITransaction
    {
        void LoadData(string[] data, User user);
        double CountTransactionPrice();
        void CountMoney();
        bool Operation();
        string State { get; set; }
        User Agent { get; set; }
        double Sum { get; set; }
    }
}
