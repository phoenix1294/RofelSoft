using RofelSoft.Enums;
using RofelSoft.Interfaces;

namespace RofelSoft.Services.News
{
    class News : IService
    {
        public string ServiceName { get; set; }

        public ServiceStatus Status { get; set; }

        public News()
        {
            ServiceName = "News";
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
