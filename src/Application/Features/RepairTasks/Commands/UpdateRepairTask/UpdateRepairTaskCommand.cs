using Domain.Common.Results;
using Domain.Features.RepairTasks;
using MediatR;

namespace Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public  record UpdateRepairTaskCommand(
     Guid RepairTaskId,
     string Name,
     decimal LaborCost,
     RepairTaskDurationInMinutes TaskDuration,
     List<UpdatePartCommand> UpdatePartCommands
 ) : IRequest<Result<Updated>>;
}