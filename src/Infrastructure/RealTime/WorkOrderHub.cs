using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.RealTime
{
    public sealed class WorkOrderHub : Hub
    {
        public const string HubUrl = "/hubs/workorders";
         private readonly ILogger<WorkOrderHub> logger;

    public WorkOrderHub(ILogger<WorkOrderHub> logger)
    {
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        logger.LogInformation(
            "SignalR client connected. ConnectionId: {ConnectionId}",
            Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation(
            exception,
            "SignalR client disconnected. ConnectionId: {ConnectionId}",
            Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
    }
}