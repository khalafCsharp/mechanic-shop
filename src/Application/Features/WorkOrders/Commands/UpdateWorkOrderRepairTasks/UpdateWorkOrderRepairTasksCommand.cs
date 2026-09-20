using Domain.Common.Results;
using MediatR;

namespace Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks
{
    public record UpdateWorkOrderRepairTasksCommand(Guid WorkOrderId,
    Guid[] RepairTaskIds): IRequest<Result<Updated>>;
}