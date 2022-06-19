using Newtonsoft.Json;
using RofelSoft.Enums;
using RofelSoft.Models;
using RofelSoft.Models.Minecraft;
using System.Linq;
using System.Text.RegularExpressions;

namespace RofelSoft.Utils
{
    static class CraftApi
    {
        public static void Say(string str, string username, Color color, bool italic = false, bool bold = false, bool obfuscated = false, bool underlined = false, bool strikethrough = false)
        {
            var text = new JsonTextComponent()
            {
                Text = str,
                Italic = italic,
                Bold = bold,
                Obfuscated = obfuscated,
                Underlined = underlined,
                Strikethrough = strikethrough,
                Color = color
            };
            RootService.Connector.Command($"/tellraw {username} {text.Serialize()}");
        } //tested

        public static void Say(JsonTextComponent text, string username)
        {
            RootService.Connector.Command($"/tellraw {username} {text.Serialize()}");
        }

        public static void Broadcast(string str, Color color, bool italic = false, bool bold = false, bool obfuscated = false, bool underlined = false, bool strikethrough = false)
        {
            Say(str, "@a", color, italic, bold, obfuscated, underlined, strikethrough);
        }

        public static void Broadcast(JsonTextComponent text)
        {
            Say(text, "@a");
        }

        public static void PlaySound(User player, string sound)
        {
            var pos = GetUserPosition(player);
            if (pos != null)
            {
                RootService.Connector.Command($"/playsound {sound} neutral @a {pos.X} {pos.Y} {pos.Z}");
            }
            else
            {
                Logger.Error("Invalid value: GetUserPosition returned null");
            }
        }

        public static bool TakeItem(User player, Item item)
        {
            var response = RootService.Connector.Command($"/clear {player.Name} {item.Serialize()}");
            return !response.Contains("No items were found");
        }

        public static bool TakeItem(User player, Product prod, int count)
        {
            var response = RootService.Connector.Command($"/clear {player.Name} {prod.ItemCode} {count}");
            var s = new Regex(@"\s\d*\s").Matches(response);
            return int.Parse(s[0].Value.Trim()) > 0;
        }

        public static int GetItemCount(User player, Product item)
        {
            try
            {
                var response = RootService.Connector.Command($"/clear {player.Name} {item.ItemCode} 0");
                var s = new Regex(@"\s\d*\s").Matches(response);
                return int.Parse(s[0].Value.Trim());
            }
            catch
            {
                return 0;
            }
        }

        public static int GetItemCount(User player, Item item)
        {
            try
            {
                var response = RootService.Connector.Command($"/clear {player.Name} {item.Serialize()[0..^2]} 0");
                var s = new Regex(@"\s\d*\s").Matches(response);
                return int.Parse(s[0].Value.Trim());
            }
            catch
            {
                return 0;
            }
        }

        public static bool GiveItem(User player, Item item)
        {
            var response = RootService.Connector.Command($"/give {player.Name} {item.Serialize()}");
            if (response.Contains("Gave"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GiveItem(User player, Product item, int count)
        {
            var response = RootService.Connector.Command($"/give {player.Name} {item.ItemCode} {count}");
            if (response.Contains("Gave"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static ItemStack[] GetUserInventory(User player)
        {
            var response = RootService.Connector.Command($"/data get entity {player.Name} Inventory");

            if (!response.Contains("No entitiy"))
            {
                try
                {
                    response = response[(response.IndexOf('[') - 1)..];
                    for (int e = 0; e < 10; e++)
                    {
                        response = response.Replace($"{e}b", $"{e}").Replace($"{e}s", $"{e}");
                    }
                    return JsonConvert.DeserializeObject<ItemStack[]>(response, RootService.Converters);
                }
                catch
                {
                    Logger.Error("Invalid response: unable to extract ItemStack[]");
                    return null;
                }
            }
            else
            {
                Logger.Error("Invalid response: player not found");
                return null;
            }
        }

        public static Item GetUserHand(User player)
        {
            var response = RootService.Connector.Command($"/data get entity {player.Name} SelectedItemSlot");
            try
            {
                var slot = int.Parse(response[(response.IndexOf(':') + 1)..]);
                var inv = GetUserInventory(player);
                if (inv != null)
                {
                    var ret = inv.Where(item => item.Slot == slot);
                    if (ret.Count() > 0)
                    {
                        return ret.Last();
                    }
                    else
                    {
                        return new ItemStack();
                    }
                }
                else
                {
                    Logger.Error("Invalid value: GetUserInventory returned null");
                    return null;
                }
            }
            catch
            {
                Logger.Error("Invalid response: unable to extract righthanded ItemStack");
                return null;
            }
        }

        public static ItemStack GetUserLeftHand(User player)
        {
            try
            {
                var inv = GetUserInventory(player);
                if (inv != null)
                {
                    var ret = inv.Where(item => item.Slot == -106);
                    if (ret.Count() > 0)
                    {
                        return ret.Last();
                    }
                    else
                    {
                        return new ItemStack();
                    }
                }
                else
                {
                    Logger.Error("Invalid value: GetUserInventory returned null");
                    return null;
                }
            }
            catch
            {
                Logger.Error("Invalid response: unable to extract lefthanded ItemStack");
                return null;
            }
        }

        public static string GetUserDimension(User player)
        {
            var response = RootService.Connector.Command($"/data get entity {player.Name} Dimension");
            if (!response.Contains("No entity"))
            {
                try
                {
                    response = response[response.IndexOf('\"')..];
                    response = response.Replace("\"", "");
                    return response;
                }
                catch
                {
                    Logger.Error("Invalid response: unable to extract dimension");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static Vector GetUserPosition(User player)
        {
            var response = RootService.Connector.Command($"/data get entity {player.Name} Pos");
            for (int e = 0; e < 10; e++)
            {
                response = response.Replace($"{e}d", $"{e}");
            }
            if (!response.Contains("No entity"))
            {
                try
                {
                    response = response[response.IndexOf('[')..];
                    var coords = response.Replace("[", "").Replace("]", "").Replace(" ", "").Split(',');
                    int x = (int)double.Parse(coords[0].Replace('.', ','));
                    int y = (int)double.Parse(coords[1].Replace('.', ','));
                    int z = (int)double.Parse(coords[2].Replace('.', ','));
                    return new Vector(x, y, z);
                }
                catch
                {
                    Logger.Error("Invalid response: unable to extract coordinates");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string[] GetUserList()
        {
            var list = RootService.Connector.Command("/list");
            try
            {
                return list[(list.IndexOf(':') + 1)..].Replace(" ", "").Split(',');
            }
            catch
            {
                Logger.Error("Invalid response: unable to extract usernames");
                return new string[0];
            }
        }

        public static void SummonCollector(int x, int y, int z)
        {
            RootService.Connector.Command($"/summon minecraft:vindicator {x} {y} {z} " +
                "{ CustomNameVisible: 1b, CustomName: '{\"text\":\"Коллектор\", \"color\":\"red\", \"bold\":\"true\"}'," +
                " HandItems:[{ id: \"minecraft:netherite_axe\", Count: 1b, " +
                "tag: { Enchantments:[{ id: \"minecraft: sharpness\", lvl: 10s}]} }], " +
                "HandDropChances:[0f, 0f], " +
                "ActiveEffects:[{ Id: 1b, Amplifier: 10b, Duration: 99990, ShowParticles: 0b}," +
                " { Id: 8b, Amplifier: 8b, Duration: 99990, ShowParticles: 0b}, " +
                "{ Id: 11b, Amplifier: 3b, Duration: 99990, ShowParticles: 0b}]}");
        }
    }
}
