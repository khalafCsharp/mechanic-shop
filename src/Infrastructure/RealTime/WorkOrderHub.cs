using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.RealTime
{
    public sealed class WorkOrderHub : Hub
    {
        public const string HubUrl = "/hubs/workorders";
    }
}