using RofelSoft.Enums;

namespace RofelSoft.Interfaces
{
    interface IService
    {
        public string ServiceName { get; }

        public ServiceStatus Status { get; set; }

        public bool StartService();

        public bool StopService();
    }
}
