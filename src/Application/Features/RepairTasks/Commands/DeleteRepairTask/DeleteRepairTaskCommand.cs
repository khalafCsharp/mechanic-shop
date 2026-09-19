using Domain.Common.Results;
using MediatR;

namespace Application.Features.RepairTasks.Commands.DeleteRepairTask
{
    public record DeleteRepairTaskCommand(Guid RepairTaskToBeDeletedId) : IRequest<Result<Deleted>>;
}