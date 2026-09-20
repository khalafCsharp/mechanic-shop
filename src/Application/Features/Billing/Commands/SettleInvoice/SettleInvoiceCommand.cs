using Domain.Common.Results;
using MediatR;

namespace Application.Features.Billing.Commands.SettleInvoice
{
    public record SettleInvoiceCommand(Guid InvoiceId) : IRequest<Result<Success>>;
}