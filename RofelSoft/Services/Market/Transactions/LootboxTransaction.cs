using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Models.Minecraft;
using RofelSoft.Utils;
using System;

namespace RofelSoft.Services.Market.Transactions
{
    class LootboxTransaction : ITransaction
    {
        public double Sum { get; set; }
        public User Agent { get; set; }
        public string State { get; set; }

        private int Count;

        private int[] Isolation = new int[27];

        public void LoadData(string[] data, User user)
        {
            Agent = user;
            Count = int.Parse(data[0]);
        }

        public double CountTransactionPrice()
        {
            Sum = 7500 * Count;
            return Sum;
        }

        private int GenerateIsolatedInt()
        {
            Random random = new Random();
            int r;
            do
            {
                r = random.Next(0, 26);
            }
            while (Isolation[r] > 0);
            Isolation[r]++;
            return r;
        }

        private void ResetIsolator()
        {
            Isolation = new int[27];
        }

        private ItemStack[] GenerateBox()
        {
            Random random = new Random();
            int itemc = random.Next(1, 3);
            var ret = new ItemStack[itemc];

            for (int z = 0; z < itemc; z++)
            {
                var loot = RootService.Loots[random.Next(0, RootService.Loots.Count)];

                ret[z] = new ItemStack()
                {
                    Count = random.Next(0, 64),
                    Slot = GenerateIsolatedInt(),
                    Id = loot.Id,
                    Tags = loot.Tags
                };
                ret[z].Tags.Display.Name.Color = Color.Aqua;
            }
            ResetIsolator();
            return ret;
        }

        public bool Operation()
        {
            if (Sum <= Agent.Money)
            {
                if (Count <= 36 & Count > 0)
                {
                    for (int z = 0; z < Count; z++)
                    {
                        var goods = GenerateBox();
                        var lootbox = new Item()
                        {
                            Id = "minecraft:chest",
                            Count = 1,
                            Tags = new Tag()
                            {
                                Display = new Display()
                                {
                                    Name = new JsonTextComponent()
                                    {
                                        Text = "Лутбокс",
                                        Color = Color.Gold,
                                        Bold = true,
                                        Italic = false
                                    },
                                    Lore = new JsonTextComponent[]
                                    {
                                    new JsonTextComponent()
                                    {
                                        Text = "Rofelsoft Lootbox®",
                                        Color = Color.Aqua,
                                        Italic = false
                                    }
                                    }
                                },
                                BlockEntityTag = new BlockEntityTag()
                                {
                                    Items = goods
                                }
                            }
                        };
                        CraftApi.GiveItem(Agent, lootbox);
                    }
                    CraftApi.Broadcast($"{RootService.MarketService.ServiceName} {Agent.Name} купил {Count} лубоксов!", Color.Aqua);
                    CraftApi.PlaySound(Agent, "minecraft:block.barrel.open");
                    State = $"Лутбоксы куплены успешно";
                    return true;
                }
                else
                {
                    State = "Нельзя купить больше 36 и меньше 1 лутбоксов";
                    return false;
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
            Agent.AddMoney(-Sum);
        }
    }
}
