using Application.Common.Interfaces;
using Application.Features.Billing.Dtos;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Domain.Features.WorkOrders.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Billing.Queries.GetINvoicePdf
{
    public class GetInvoicePdfQueryHandler : IRequestHandler<GetInvoicePdfQuery, Result<InvoicePdfDto>>
    {
        private readonly ILogger<GetInvoicePdfQueryHandler> logger;
        private readonly IAppDbContext context;
        private readonly IInvoicePdfGenerator pdfGenerator;

        public GetInvoicePdfQueryHandler(ILogger<GetInvoicePdfQueryHandler> logger, IAppDbContext context, IInvoicePdfGenerator pdfGenerator)
        {
            this.pdfGenerator = pdfGenerator;
            this.context = context;
            this.logger = logger;
        }
        public async Task<Result<InvoicePdfDto>> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
        {
            Invoice? invoice = await context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                logger.LogWarning("invoice with id: {invoiceId} was not found", request.InvoiceId);

                return Error.NotFound("invoice not found");
            }
            try
            {
                Byte[] invoicePdfBytes = pdfGenerator.Generate(invoice);
                return new InvoicePdfDto()
                {
                    Content = invoicePdfBytes,
                    FileName = $"invoice_{invoice.Id}.pdf",
                    ContentType = "application/pdf"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate PDF for InvoiceId: {InvoiceId}", request.InvoiceId);
                
                return Error.Failure("An error occurred while generating the invoice PDF.");
            }
        }
    }
}