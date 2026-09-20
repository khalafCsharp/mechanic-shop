using Domain.Common.Results;
using Domain.Features.WorkOrders;
using MediatR;

namespace Application.Features.WorkOrders.Commands.UpdateWorkOrderState
{
    public record UpdateWorkOrderStateCommand(Guid WorkOrderId, WorkOrderState NewState) : IRequest<Result<Updated>>;
}
