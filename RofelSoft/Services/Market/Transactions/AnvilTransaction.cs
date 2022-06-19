using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Models.Minecraft;
using RofelSoft.Utils;
using System;
using System.Collections.Generic;

namespace RofelSoft.Services.Market.Transactions
{
    class AnvilTransaction : ITransaction
    {
        public User Agent { get; set; }
        public double Sum { get; set; }
        public string State { get; set; }

        private Item CacheStack;
        private Item RightStack;
        private Item LeftStack;

        private List<Enchantment> EnchantList;
        private readonly double Multiplier = 2400;

        public void CountMoney()
        {
            Agent.AddMoney(-Sum);
        }

        private Enchantment[] GetActualEnchants(Item item)
        {
            if (item.Tags != null)
            {
                if (item.Id.Contains("enchanted_book"))
                {
                    return item.Tags.StoredEnchantments ?? (new Enchantment[1] { new Enchantment() { Id = Enchant.Dummy, Level = 0 } });
                }
                else
                {
                    return item.Tags.Enchantments ?? (new Enchantment[1] { new Enchantment() { Id = Enchant.Dummy, Level = 0 } });
                }
            }
            else
            {
                return new Enchantment[1] { new Enchantment() { Id = Enchant.Dummy, Level = 0 } };
            }
        }

        public double CountTransactionPrice()
        {
            if (RightStack.Id + LeftStack.Id != "")
            {
                if (LeftStack.Id.Contains("enchanted_book"))
                {
                    EnchantList = new List<Enchantment>();
                    bool first = true;
                    var right_enchants = GetActualEnchants(RightStack);
                    var left_enchants = GetActualEnchants(LeftStack);
                    foreach (Enchantment h in right_enchants)
                    {
                        foreach (Enchantment e in left_enchants)
                        {
                            if (h.Id == e.Id)
                            {
                                if (h.Level == e.Level)
                                {
                                    h.Level++;
                                }
                                else
                                {
                                    h.Level = Math.Max(h.Level, e.Level);
                                }
                            }
                            else
                            {
                                if (first)
                                {
                                    bool add = true;
                                    for (int z = 0; z < right_enchants.Length; z++)
                                    {
                                        if (right_enchants[z].Id == e.Id)
                                        {
                                            add = false;
                                        }
                                    }
                                    if (add)
                                    {
                                        EnchantList.Add(e);
                                    }
                                }
                            }
                        }
                        EnchantList.Add(h);
                        first = false;
                    }
                    for (int z = 0; z < EnchantList.Count; z++)
                    {
                        if (RightStack.Tags != null)
                        {
                            if (RightStack.Tags.Damage > 200000 & EnchantList[z].Id.ToString().Contains("mending"))
                            {
                                CraftApi.Broadcast($"{RootService.MarketService.ServiceName} {Agent.Name} блядский инженер! Кого наебать решил?", Color.Aqua);
                                Sum = 0;
                            }
                        }

                        if (EnchantList[z].Id == Enchant.Dummy)
                        {
                            EnchantList.Remove(EnchantList[z]);
                        }
                        else
                        {
                            Sum += EnchantList[z].Level;
                        }
                    }
                    Sum *= Multiplier;
                    return Sum;
                }
                else
                {
                    State = "В левой руке должна быть зачарованная книга!";
                    return 0d;
                }
            }
            else
            {
                State = "В одной из рук пусто!";
                return 0d;
            }
        }

        public void LoadData(string[] data, User user)
        {
            Agent = user;
            LeftStack = CraftApi.GetUserLeftHand(Agent);
            RightStack = CraftApi.GetUserHand(Agent);
            CacheStack = CraftApi.GetUserHand(Agent);
        }

        public bool Operation()
        {
            if (Sum == 0)
            {
                return false;
            }
            if (Agent.Money <= Sum)
            {
                State = "Недостаточно пикселей";
                return false;
            }
            else
            {
                var outstack = new Item(RightStack.Id, 1, new Tag());
                if (RightStack.Id + LeftStack.Id != "")
                {
                    if (outstack.Id.Contains("enchanted_book"))
                    {
                        outstack.Tags.StoredEnchantments = EnchantList.ToArray();
                    }
                    else
                    {
                        if (RightStack.Tags != null)
                        {
                            if (RightStack.Tags.Display != null)
                            {
                                outstack.Tags.Display = RightStack.Tags.Display;
                            }
                            if (RightStack.Tags.Damage > 0 & RightStack.Id == LeftStack.Id)
                            {
                                outstack.Tags.Damage = RightStack.Tags.Damage - LeftStack.Tags.Damage;
                            }
                            else
                            {
                                outstack.Tags.Damage = RightStack.Tags.Damage;
                            }
                        }
                        outstack.Tags.Enchantments = EnchantList.ToArray();
                    }
                    var a = CraftApi.GetItemCount(Agent, LeftStack);
                    var b = CraftApi.GetItemCount(Agent, CacheStack);
                    if (a * b > 0)
                    {
                        var c = CraftApi.TakeItem(Agent, LeftStack);
                        var d = CraftApi.TakeItem(Agent, CacheStack);
                        if (c & d)
                        {
                            CraftApi.PlaySound(Agent, "minecraft:block.anvil.use");
                            CraftApi.GiveItem(Agent, outstack);
                            State = $"Успешно сложено за {Sum} пикселей";
                            return true;
                        }
                        else
                        {
                            State = "Ошибка при изъятии предметов";
                            return false;
                        }
                    }
                    else
                    {
                        State = "В какой-то руке пусто";
                        return false;
                    }
                }
                else
                {
                    State = "Один из предметов в руках не подходит";
                    return false;
                }
            }
        }
    }
}