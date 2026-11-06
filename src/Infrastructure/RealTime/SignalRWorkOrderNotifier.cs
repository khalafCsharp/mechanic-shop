using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.RealTime
{
    public class SignalRWorkOrderNotifier : IWorkOrderNotifier
    {
        private readonly ILogger<SignalRWorkOrderNotifier> logger;
        public SignalRWorkOrderNotifier(IHubContext<WorkOrderHub> hubContext, ILogger<SignalRWorkOrderNotifier> logger)
        {
            this.logger = logger;
            this.hubContext = hubContext;
        }
        private readonly IHubContext<WorkOrderHub> hubContext;
        public async Task NotifyWorkOrdersChangedAsync(
     CancellationToken ct = default)
        {
            logger.LogError("Sending WorkOrdersChanged");

            await hubContext.Clients.All.SendAsync(
                "WorkOrdersChanged",
                cancellationToken: ct);

            logger.LogError("WorkOrdersChanged sent successfully");
        }

        public async Task NotifyInvoiceIssuedAsync(CancellationToken ct = default)
        {
            logger.LogError("Sending invoiceIssued");

            await hubContext.Clients.All.SendAsync(
                "InvoiceIssued",
                cancellationToken: ct);

            logger.LogError("WorkOrdersChanged sent successfully");
        }
    }
}
