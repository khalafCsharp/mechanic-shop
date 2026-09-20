using Application.Features.Billing.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.Billing.Queries.GetINvoicePdf
{
    public record GetInvoicePdfQuery(Guid InvoiceId) : IRequest<Result<InvoicePdfDto>>;
}