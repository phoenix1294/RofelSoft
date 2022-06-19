using Newtonsoft.Json;
using RofelSoft.Models;
using System.Collections.Generic;
using System.IO;

namespace RofelSoft.Utils
{
    class DataHelper
    {
        public static void ReadAll()
        {
            ReadUsers();
            ReadSettings();
            ReadGoods();
        }

        public static void WriteAll()
        {
            WriteUsers();
            WriteSettings();
            WriteGoods();
        }

        public static void WriteSettings()
        {
            File.WriteAllText("Resources/settings.json", JsonConvert.SerializeObject(RootService.Settings, Formatting.Indented));
        }

        public static void WriteGoods()
        {
            File.WriteAllText("Resources/goods.json", JsonConvert.SerializeObject(RootService.Goods, Formatting.Indented));
        }

        public static void ReadGoods()
        {
            try
            {
                var settings = File.ReadAllText("Resources/goods.json");
                RootService.Goods = JsonConvert.DeserializeObject<List<Product>>(settings);
            }
            catch
            {
                Logger.Error("settings file not exist or corrupted");
                RootService.Goods = new List<Product>();
            }
        }

        public static void ReadUsers()
        {
            try
            {
                var settings = File.ReadAllText("Resources/users.json");
                RootService.Users = JsonConvert.DeserializeObject<List<User>>(settings);
            }
            catch
            {
                Logger.Error("settings file not exist or corrupted");
                RootService.Users = new List<User>();
            }
        }

        public static void WriteUsers()
        {
            File.WriteAllText("Resources/users.json", JsonConvert.SerializeObject(RootService.Users, Formatting.Indented));
        }

        public static void ReadSettings()
        {
            try
            {
                var settings = File.ReadAllText("Resources/settings.json");
                RootService.Settings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            catch
            {
                Logger.Error("settings file not exist or corrupted");
                RootService.Settings = new Settings();
            }

        }
    }
}
