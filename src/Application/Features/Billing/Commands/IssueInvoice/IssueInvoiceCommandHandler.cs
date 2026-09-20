using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Features.Billing.Dtos;
using Application.Features.Billing.Mappers;
using Domain.Common.Constants;
using Domain.Common.Results;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.Billing.Commands.IssueInvoice
{
    public class IssueInvoiceCommandHandler : IRequestHandler<IssueInvoiceCommand, Result<InvoiceDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<IssueInvoiceCommandHandler> logger;
        private readonly HybridCache cache;
        private readonly TimeProvider dateTime;

        public IssueInvoiceCommandHandler(IAppDbContext context, ILogger<IssueInvoiceCommandHandler> logger, HybridCache cache, TimeProvider dateTime)
        {
            this.context = context;
            this.logger = logger;
            this.cache = cache;
            this.dateTime = dateTime;
        }
        public async Task<Result<InvoiceDto>> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
        {
            WorkOrder? workOrder = await context.WorkOrders.Include(wo => wo.Vehicle).ThenInclude(v => v!.Customer).Include(wo => wo.RepairTasks).ThenInclude(rt => rt.Parts).FirstOrDefaultAsync(wo => wo.Id == request.WorkOrderId, cancellationToken);

            if (workOrder is null)
            {
                logger.LogWarning("work order with id: {workOrderId} not found", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }
            if (workOrder.State != WorkOrderState.Completed)
            {
                logger.LogWarning("work order with id: {workOrderId} not completed yet, No invoice would be issued for it.", request.WorkOrderId);

                return ApplicationErrors.WorkOrderMustBeCompletedForInvoicing;
            }
            Guid invoiceId = Guid.NewGuid();
            List<InvoiceLineItem> lineItems = [];
            int lineNumber = 1;

            foreach (var (task, taskIndex) in workOrder.RepairTasks.Select((task, taskIndex) => (task, taskIndex + 1)))
            {
                var partsSummary = task.Parts.Any()
              ? string.Join(Environment.NewLine, task.Parts.Select(p => $"    • {p.Name} x{p.Quantity} @ {p.Cost:C}"))
              : "    • No parts";

                var lineDescription =
                  $"{taskIndex}: {task.Name}{Environment.NewLine}" +
                  $"  Labor = {task.LaborCost:C}{Environment.NewLine}" +
                  $"  Parts:{Environment.NewLine}{partsSummary}";

                var totalPartsCost = task.Parts.Sum(p => p.Cost * p.Quantity);
                var totalTaskCost = task.LaborCost + totalPartsCost;

                var lineItemResult = InvoiceLineItem.Create(
                invoiceId: invoiceId,
                lineNumber: lineNumber++,
                description: lineDescription,
                quantity: 1,
                unitPrice: totalTaskCost);

                if (lineItemResult.IsError)
                {
                    return lineItemResult.Errors;
                }

                lineItems.Add(lineItemResult.Value);


            }

            var subtotal = lineItems.Sum(x => x.LineTotal);

            var taxAmount = subtotal * SystemConstants.TaxRate;

            var discountAmount = workOrder.Discount ?? 0m;

            var createInvoiceResult = Invoice.Create(
                id: invoiceId,
                workOrderId: workOrder.Id,
                items: lineItems,
                discountAmount: discountAmount,
                taxAmount: taxAmount,
                datetime: dateTime);

            if (createInvoiceResult.IsError)
            {
                logger.LogWarning(
                     "Invoice creation failed for WorkOrderId: {WorkOrderId}. Errors: {@Errors}",
                     request.WorkOrderId,
                     createInvoiceResult.Errors);

                return createInvoiceResult.Errors;
            }
            var invoice = createInvoiceResult.Value;

            await context.Invoices.AddAsync(invoice, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("invoices", cancellationToken);

            logger.LogInformation("Invoice {InvoiceId} issued for WorkOrder {WorkOrderId}.", invoice.Id, workOrder.Id);

            return invoice.ToDto();
        }
    }
}