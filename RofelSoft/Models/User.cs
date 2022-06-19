using Newtonsoft.Json;

namespace RofelSoft.Models
{
    class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public double Money { get; set; }
        public byte Permission { get; set; }

        [JsonIgnore]
        public bool Authorized { get; set; }
        [JsonIgnore]
        public bool IngameWait { get; set; }

        public User(string name = "", string password = "", double money = 0, byte permission = 0x00)
        {
            Name = name;
            Password = password;
            Money = money;
            Permission = permission;
        }
        public void AddMoney(double sum)
        {
            Money += sum;
        }
    }
}
