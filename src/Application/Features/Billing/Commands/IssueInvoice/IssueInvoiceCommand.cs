using Application.Features.Billing.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.Billing.Commands.IssueInvoice
{
    public record IssueInvoiceCommand(Guid WorkOrderId) : IRequest<Result<InvoiceDto>>;
}