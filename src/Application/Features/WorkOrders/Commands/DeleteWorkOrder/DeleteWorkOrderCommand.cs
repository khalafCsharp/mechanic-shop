using Domain.Common.Results;
using MediatR;

namespace Application.Features.WorkOrders.Commands.DeleteWorkOrder
{
    public record DeleteWorkOrderCommand(Guid WorkOrderId): IRequest<Result<Deleted>>;
}