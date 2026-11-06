using Application.Common.Interfaces;
using Domain.Features.WorkOrders.Billing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Billing.EventHandlers
{
    public class InvoiceIssuedEventHandler:INotificationHandler<InvoiceIssued>
    {
        public InvoiceIssuedEventHandler(IWorkOrderNotifier notifier, ILogger<InvoiceIssuedEventHandler> logger)
        {
            this.notifier = notifier;
            this.logger = logger;
        }
        private readonly ILogger<InvoiceIssuedEventHandler> logger;
         private readonly IWorkOrderNotifier notifier;
          public Task Handle(InvoiceIssued notification, CancellationToken cancellationToken)
        {
            logger.LogError("we are about to call the signalr hub method");
           return notifier.NotifyInvoiceIssuedAsync(cancellationToken);
        }
    }
}