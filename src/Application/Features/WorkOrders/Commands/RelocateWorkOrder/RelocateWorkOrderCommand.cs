using Domain.Common.Results;
using Domain.Features.WorkOrders.Enums;
using MediatR;

namespace Application.Features.WorkOrders.Commands.RelocateWorkOrder
{
    public record RelocateWorkOrderCommand(Guid WorkOrderId,
    DateTimeOffset NewStartAt,
    Spot NewSpot): IRequest<Result<Updated>>;
}