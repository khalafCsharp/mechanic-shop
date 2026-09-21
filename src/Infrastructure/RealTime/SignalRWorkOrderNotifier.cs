using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.RealTime
{
    public class SignalRWorkOrderNotifier : IWorkOrderNotifier
    {
        public SignalRWorkOrderNotifier(IHubContext<WorkOrderHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        private readonly IHubContext<WorkOrderHub> hubContext;
        public Task NotifyWorkOrdersChangedAsync(CancellationToken ct = default)
        {
           return hubContext.Clients.All.SendAsync("WorkOrdersChanged",cancellationToken: ct);
        }
    }
}