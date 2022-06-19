using System.IO;

namespace RofelSoft.Services.Market
{
    static class WebPages
    {
        public static string LoginPage
        {
            get
            {
                return File.ReadAllText("Resources/Web/login.htm").Replace("{CURRENT_HOST}", RootService.Settings.WebHost);
            }
        }

        public static string MarketPage
        {
            get
            {
                return File.ReadAllText("Resources/Web/market.htm").Replace("{CURRENT_HOST}", RootService.Settings.WebHost);
            }
        }

        public static string NotFoundPage
        {
            get
            {
                return File.ReadAllText("Resources/Web/notfound.htm");
            }
        }
    }
}
