using Newtonsoft.Json;
using RofelSoft.Enums;
using RofelSoft.Interfaces;
using RofelSoft.Models;
using RofelSoft.Services.Market.Enums;
using RofelSoft.Services.Market.Models;
using RofelSoft.Services.Market.Transactions;
using RofelSoft.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RofelSoft.Services.Market
{
    class Market : IService
    {
        public string ServiceName { get; set; }

        public ServiceStatus Status { get; set; }

        private TcpListener Listener { get; set; }

        public Market()
        {
            Listener = new TcpListener(IPAddress.Any, 80);
            ServiceName = "Market";
        }

        public bool StartService()
        {
            Listener.Start();
            Status = ServiceStatus.Running;
            new Thread(HandlingThread).Start();
            return true;
        }

        private void HandlingThread()
        {
            while (Status == ServiceStatus.Running)
            {
                var soc = Listener.AcceptSocket();
                Task.Run(() =>
                {
                    try
                    {
                        HandleRequest(soc);
                    }
                    catch(Exception e)
                    {
                        Logger.Error(e.Message);
                    }
                });
            }
        }

        private User FastFind(string login, string password)
        {
            int idx = -1;
            for (int s = 0; s < RootService.Users.Count; s++)
            {
                if (RootService.Users[s].Password == password)
                {
                    idx = s;
                    break;
                }
            }
            if (idx > -1)
            {
                for (int s = idx; s < RootService.Users.Count; s++)
                {
                    if (RootService.Users[s].Name == login & idx == s)
                    {
                        return RootService.Users[s];
                    }
                }
            }
            return null;
        } // very fast 

        private bool CheckUserAuth(out User player, string login = "", string password = "", string cookies = "")
        {
            player = FastFind(login, password);
            if (player == null)
            {
                var cs = cookies.Split(';');
                string cookie_login = "";
                string cookie_pass = "";
                for (int z = 0; z < cs.Length; z++)
                {
                    var k = cs[z].Split('=');
                    if (k[0] == " name")
                    {
                        cookie_login = k[1];
                    }
                    if (k[0] == " key")
                    {
                        cookie_pass = k[1];
                    }
                }

                player = FastFind(cookie_login, cookie_pass);
                return player != null;
            }
            else
            {
                return true;
            }
        }

        private bool CheckUserAuth(string login = "", string password = "", string cookies = "")
        {
            return CheckUserAuth(out _, login, password, cookies);
        }

        public double GetTransactionPrice(ITransaction action, User agent, HttpRequest req)
        {
            var args = new string[8];
            for (int z = 0; z < 8; z++)
            {
                args[z] = req.ExtractHeader($"Arg{z}");
            }
            action.LoadData(args, agent);
            return action.CountTransactionPrice();
        }

        public static string ExecuteTransaction(ITransaction action, User current, HttpRequest req)
        {
            if (CraftApi.GetUserDimension(current) != null)
            {
                try
                {
                    var args = new string[8];
                    for (int z = 0; z < 8; z++)
                    {
                        args[z] = req.ExtractHeader($"Arg{z}");
                    }
                    action.LoadData(args, current);
                    action.CountTransactionPrice();
                    if (action.Operation())
                    {
                        action.CountMoney();
                        DataHelper.WriteGoods();
                        DataHelper.WriteUsers();
                    }
                    return action.State;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            else
            {
                return "Игрока нет на сервере!";
            }
        }

        private void ExecuteIfAccepted(Socket client, User CurrentUser, HttpRequest request, ITransaction transaction)
        {
            if (request.ExtractHeader("Accepted") == "true")
            {
                var res = ExecuteTransaction(transaction, CurrentUser, request);
                SendPage(client, $"{{\"state\": \"{res}\"}}");
            }
            else
            {
                var res = GetTransactionPrice(transaction, CurrentUser, request);
                SendPage(client, $"{{\"price\": \"{res}\"}}");
            }
        }

        private void HandleRequest(Socket client)
        {
            var buf = new byte[65536];
            var len = client.Receive(buf);
            var request = new HttpRequest(buf, len);
            if (request.Type == "GET")
            {
                switch (request.Path)
                {
                    case "/":
                        {
                            SendPage(client, WebPages.LoginPage);
                            break;
                        }
                    case "/market":
                        {
                            if (CheckUserAuth(cookies: request.ExtractHeader("Cookie")))
                            {
                                SendPage(client, WebPages.MarketPage);
                            }
                            else
                            {
                                SendPage(client, WebPages.LoginPage);
                            }
                            break;
                        }
                    default:
                        {
                            try
                            {
                                var bytes = File.ReadAllBytes($"Resources/Web{request.Path}");
                                SendRaw(client, bytes, ResolveType(request.Path));
                            }
                            catch
                            {
                                SendPage(client, WebPages.NotFoundPage);
                            }
                            break;
                        }
                }
            }
            else
            {
                switch (request.Path)
                {
                    case "/login":
                        {
                            var login = request.ExtractHeader("Login");
                            var pass = request.ExtractHeader("Password");
                            if (CheckUserAuth(login: login, password: pass))
                            {
                                SendPage(client, "{ \"state\": \"ok\" }");
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"invalid login or password\" }");
                            }
                            break;
                        }
                    case "/register":
                        {
                            var login = request.ExtractHeader("Login");
                            var pass = request.ExtractHeader("Password");

                            if (RootService.Users.Where(e => e.Name == login).Count() > 0)
                            {
                                SendPage(client, "{ \"state\": \"user already exists\" }");
                            }
                            else
                            {
                                if (login.Length > 2 & pass.Length > 2)
                                {
                                    RootService.Users.Add(new User()
                                    {
                                        Name = login,
                                        Password = pass,
                                        Money = 0.00f,
                                        Permission = 0x00
                                    });
                                    SendPage(client, "{ \"state\": \"ok\" }");
                                }
                                else
                                {
                                    SendPage(client, "{ \"state\": \"too short login or password\" }");
                                }
                            }
                            break;
                        }
                    case "/price":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                var response = new StringBuilder();
                                response.Append($"{JsonConvert.SerializeObject(CurrentUser)}&");
                                response.Append($"{JsonConvert.SerializeObject(RootService.Goods)}&");
                                response.Append($"{RootService.GetUserList(CurrentUser)}&");
                                response.Append($"{{ " +
                                    $"\"priceCorrector\":{RootService.PriceFactor}," +
                                    $"\"reductionBorder\":{RootService.ReductionBorder}," +
                                    $"\"historyInterval\":{RootService.TickInterval}" +
                                    $"}}");
                                SendPage(client, $"{response}");
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/update":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                var id = int.Parse(request.ExtractHeader("Id"));
                                var response = new StringBuilder();
                                response.Append($"{JsonConvert.SerializeObject(CurrentUser)}&");
                                response.Append($"{JsonConvert.SerializeObject(RootService.Goods[id])}&");
                                response.Append($"{RootService.GetUserList(CurrentUser)}&");
                                SendPage(client, $"{response}");
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/ic":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                var id = int.Parse(request.ExtractHeader("Id"));
                                var count = CraftApi.GetItemCount(CurrentUser, RootService.Goods[id]);
                                SendPage(client, $"{{ \"count\": \"{count}\" }}");
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/sell":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                ExecuteIfAccepted(client, CurrentUser, request, new SellTransaction());
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/buy":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                ExecuteIfAccepted(client, CurrentUser, request, new BuyTransaction());
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/tp":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                ExecuteIfAccepted(client, CurrentUser, request, new TpTransaction());
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/transfer":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                var res = ExecuteTransaction(new TransferTransaction(), CurrentUser, request);
                                SendPage(client, $"{{\"state\": \"{res}\"}}");
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/anvil":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                ExecuteIfAccepted(client, CurrentUser, request, new AnvilTransaction());
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/lootbox":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                ExecuteIfAccepted(client, CurrentUser, request, new LootboxTransaction());
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/xp":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                ExecuteIfAccepted(client, CurrentUser, request, new XpTransaction());
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/com":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                if (CurrentUser.Permission > 0x01)
                                {
                                    var com = request.ExtractHeader("Text");
                                    SendPage(client, RootService.AdminService.Execute(com));
                                }
                                else
                                {
                                    SendPage(client, "{ \"state\": \"not enough permissions\" }");
                                }
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/reset":
                        {
                            
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                if (CurrentUser.Authorized)
                                {
                                    CurrentUser.Authorized = false;
                                    CurrentUser.IngameWait = false;
                                    RootService.FirewallService.ActualList.Remove(CurrentUser.Name);
                                    Logger.Log(CurrentUser.Name + " Resetted");
                                }
                                SendPage(client, "{\"state\":\"Успешно сброшено. Вас выкинуло с сервера\"}");
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    case "/prove":
                        {
                            if (CheckUserAuth(out User CurrentUser, cookies: request.ExtractHeader("Cookie")))
                            {
                                if (!CurrentUser.Authorized)
                                {
                                    CurrentUser.Authorized = true;
                                    CurrentUser.IngameWait = true;
                                    RootService.FirewallService.ActualList.Add(CurrentUser.Name);
                                    Logger.Log(CurrentUser.Name + " Approved");
                                }
                                SendPage(client, "{\"state\":\"Успешно подтверждено\"}");
                            }
                            else
                            {
                                SendPage(client, "{ \"state\": \"not authorized\" }");
                            }
                            break;
                        }
                    default:
                        {
                            SendPage(client, "{ \"state\": \"unknown\" }");
                            break;
                        }
                }
            }
            client.Close();
        }

        private string ResolveType(string path)
        {
            switch (path.Split('.')[^1])
            {
                case "js":
                    {
                        return "text/javascript";
                    }
                case "css":
                    {
                        return "text/css";
                    }
                default:
                    {
                        return "application/octet-stream";
                    }
            }
        }

        private void SendPage(Socket client, string page)
        {
            client.Send(new HttpResponse(page, HttpStatus.OK).Serialize());
        }

        private void SendRaw(Socket client, byte[] bytes, string ctype)
        {
            client.Send(new RawResponse(bytes, ctype, HttpStatus.OK).Serialize());
        }

        public bool StopService()
        {
            Status = ServiceStatus.Stopped;
            return true;
        }
    }
}
