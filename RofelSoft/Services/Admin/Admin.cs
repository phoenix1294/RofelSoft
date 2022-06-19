using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Models.Minecraft;
using RofelSoft.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RofelSoft.Services.Admin
{
    class Command
    {
        public Func<string, string> Exec { get; set; }
        public string HelpString { get; set; }
    }

    class Admin : IService
    {
        private int ProductsByAlphabet(Product a, Product b)
        {
            var nameA = a.ItemName;
            var nameB = b.ItemName;
            for (int z = 0; z < Math.Min(nameA.Length, nameB.Length); z++)
            {
                if (nameA[z] > nameB[z])
                {
                    return 1;
                }
                else
                {
                    if (nameA[z] < nameB[z])
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }

        private readonly Dictionary<string, Command> Registry = new Dictionary<string, Command>();

        public string ServiceName { get; set; }

        public ServiceStatus Status { get; set; }

        public Admin()
        {
            ServiceName = "Admin";
            Initialize();
        }

        public void Initialize()
        {   
            Registry.Add("config.load", new Command()
            {
                Exec = (arg) =>
                {
                    DataHelper.ReadAll();
                    return "Configuration loaded from disk";
                },
                HelpString = "Loads configuration from disk"
            });
            
            Registry.Add("config.drop", new Command()
            {
                Exec= (arg) =>
                {
                    DataHelper.WriteAll();
                    return "Configuration saved to disk";
                },
                HelpString = "Unloads config to disk"
            });
            Registry.Add("price.reset", new Command()
            {
                Exec = (arg) =>
                {
                    foreach (Product prod in RootService.Goods)
                    {
                        prod.ActualPrice = prod.BeginPrice;
                    }
                    DataHelper.WriteAll();
                    return $"{RootService.Goods.Count} items price was returned to BeginPrice";
                },
                HelpString = "Resets the price in all goods"
            });
            Registry.Add("price.hreset", new Command() 
            {
                Exec = (arg) =>
                {
                    foreach (Product prod in RootService.Goods)
                    {
                        prod.PriceHistory = new List<double>();
                    }
                    DataHelper.WriteAll();
                    return $"{RootService.Goods.Count} items price history is []";
                },
                HelpString = "Resets the history price for goods"
            });
            Registry.Add("user.create", new Command()
            {
                Exec = (arg) =>
                {
                    var args = arg.Split(' ');
                    RootService.Users.Add(new User() { Name = args[0], Money = 0, Password = args[1], Permission = byte.Parse(args[2]) });
                    DataHelper.WriteUsers();
                    return $"User {args[0]} was added";
                },
                HelpString = "Adds the new user: user.create <username> <password> <permission>"
            });
            Registry.Add("user.remove", new Command()
            {
                Exec = (arg) =>
                {
                    var sum = RootService.Users.Where((e) => { return e.Name == arg; }).Last().Money;
                    var removed = RootService.Users.RemoveAll((e) => { return e.Name == arg; });
                    if (removed > 0)
                    {
                        sum = Math.Round(sum / RootService.Users.Count, 2);
                        if (sum > 0)
                        {
                            foreach (User p in RootService.Users)
                            {
                                p.Money += sum;
                                CraftApi.Say($"[RofelSoft] Поздравляю, вы унаследовали {sum} пикселей от {arg}", p.Name, Color.Gold);
                            }
                        }
                        DataHelper.WriteUsers();
                        return $"{removed} users with name {arg} removed";
                    }
                    else
                    {
                        return $"user with name {arg} not found";
                    }
                },
                HelpString = "Removes user: user.remove <username>"
            });
            Registry.Add("user.take", new Command()
            {
                Exec = (arg) =>
                {
                    var args = arg.Split(' ');
                    if (args.Length < 3)
                    {
                        return "Not enough args";
                    }
                    else
                    {
                        var success = CraftApi.TakeItem(new User(name: args[0]), new ItemStack(args[1], int.Parse(args[2])));
                        return success ? $"{args[2]} items taken from {args[0]}" : $"{args[1]} not found in {arg[0]}";
                    }
                },
                HelpString = "Takes an item from user: user.take <username> <itemcode> <quantity>"
            });
            /*
            Registry.Add("user.hand", (arg) =>
            {
                var stack = CraftApi.GetPlayerHand(new Player() { Name = arg });
                return $"Id: {stack.Id}, Count: {stack.Count}";
            });
            Registry.Add("user.inventory", (arg) =>
            {
                var inv = CraftApi.GetPlayerInventory(new Player() { Name = arg });
                var sb = new StringBuilder();
                foreach (ItemStack e in inv)
                {
                    sb.Append($"Slot: {e.Slot}, Item: {e.Id}, Tags: {e.Tags.Serialize()}, Count: {e.Count}");
                }
                return sb.ToString();
            });
            */
            Registry.Add("rcon.exec", new Command()
            {
                Exec = (arg) =>
                {
                    return RootService.Connector.Command(arg);
                },
                HelpString = "Executes commant via RCON"
            });
            Registry.Add("price.sort", new Command()
            {
                Exec = (arg) =>
                {
                    RootService.Goods.Sort(ProductsByAlphabet);
                    return "Items sorted";
                },
                HelpString = "Sorts all products"
            });
            /*
            Registry.Add("price.fixids", (arg) =>
            {
                var cnt = RootService.Goods.Count;
                for (int i = 0; i < cnt; i++)
                {
                    Logger.Log($"{RootService.Goods[i].ItemName}: {RootService.Goods[i].Id} -> {i}");
                    RootService.Goods[i].Id = i;
                }
                return "Products fixed!";
            });
            Registry.Add("price.import", (arg) =>
            {
                try
                {
                    var lines = File.ReadAllLines(arg);
                    RootService.Goods = new List<Product>();
                    var sb = new StringBuilder();
                    for (int i = 2; i < lines.Length; i++)
                    {
                        var lint = lines[i].Split(',');
                        var name = lint[0];
                        var itemcode = lint[5];
                        var price = double.Parse($"{lint[6]},{lint[7]}".Replace("\"", ""));

                        RootService.Goods.Add(new Product()
                        {
                            BeginPrice = price,
                            ActualPrice = price,
                            Id = i + 3,
                            ItemCode = itemcode,
                            ItemName = name,
                            FirstSold = false,
                            StaticPrice = false
                        });

                        sb.Append($"{name} {itemcode} {price} imported");
                    }
                    Configurator.DropConfig();
                    return sb.ToString();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            });
            */
        }

        public string Execute(string inbox)
        {
            if (inbox.Length == 0) { return ""; }
            var idx = inbox.IndexOf(' ');
            var com = idx < 1 ? inbox : inbox.Substring(0, idx);
            var arg = idx < 1 ? "" : inbox.Substring(idx + 1, inbox.Length - idx - 1);

            if (Registry.ContainsKey(com))
            {
                return Registry[com].Exec(arg);
            }
            else
            {
                if (com == "help")
                {
                    var sb = new StringBuilder();
                    foreach (string e in Registry.Keys)
                    {
                        sb.Append($"{e}\n");
                    }
                    return sb.ToString();
                }
                else
                {
                    return "Unknown command";
                }
            }
        }

        public bool StartService()
        {
            return true;
        }

        public bool StopService()
        {
            return true;
        }
    }
}
