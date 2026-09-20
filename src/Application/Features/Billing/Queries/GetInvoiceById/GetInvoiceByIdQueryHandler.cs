using Application.Common.Interfaces;
using Application.Features.Billing.Dtos;
using Application.Features.Billing.Mappers;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Domain.Features.WorkOrders.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Billing.Queries.GetInvoiceById
{
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<GetInvoiceByIdQueryHandler> logger;
        public GetInvoiceByIdQueryHandler(IAppDbContext context, ILogger<GetInvoiceByIdQueryHandler> logger)
        {
            this.logger = logger;
            this.context = context;

        }
        public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            Invoice? invoice = await context.Invoices.AsNoTracking().Include(i => i.LineItems).Include(i => i.WorkOrder).ThenInclude(wo => wo.Vehicle).ThenInclude(v => v.Customer).FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                logger.LogWarning("invoice for id: {invoiceId} not found", request.InvoiceId);

                return Error.NotFound("Invoice not found");
            }
            return invoice.ToDto();
        }
    }
}