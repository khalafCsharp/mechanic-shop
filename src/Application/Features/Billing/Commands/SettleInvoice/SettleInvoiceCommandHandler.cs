using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Features.WorkOrders.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.Billing.Commands.SettleInvoice
{
    public class SettleInvoiceCommandHandler : IRequestHandler<SettleInvoiceCommand, Result<Success>>
    {
        private readonly ILogger<SettleInvoiceCommandHandler> logger;
        private readonly HybridCache cache;
        private readonly IAppDbContext context;
        private readonly TimeProvider dateTime;

        public SettleInvoiceCommandHandler(ILogger<SettleInvoiceCommandHandler> logger, HybridCache cache, IAppDbContext context, TimeProvider dateTime)
        {
            this.dateTime = dateTime;
            this.cache = cache;
            this.context = context;
            this.logger = logger;
        }
        public async Task<Result<Success>> Handle(SettleInvoiceCommand request, CancellationToken cancellationToken)
        {
            Invoice? invoice = await context.Invoices.FirstOrDefaultAsync(i=>i.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                logger.LogWarning("Invoice {InvoiceId} not found.", request.InvoiceId);
                return ApplicationErrors.InvoiceNotFound;
            }

            Result<Updated> payInvoiceResult = invoice.MarkAsPaid(dateTime);

            if (payInvoiceResult.IsError)
            {
                logger.LogWarning(
                   "Invoice payment failed for InvoiceId: {InvoiceId}. Errors: {Errors}",
                   invoice.Id,
                   payInvoiceResult.Errors);

                return payInvoiceResult.Errors;
            }

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("invoices", cancellationToken);

            logger.LogInformation("Invoice {InvoiceId} successfully paid.", invoice.Id);

            return Result.Success;
        }
    }
}