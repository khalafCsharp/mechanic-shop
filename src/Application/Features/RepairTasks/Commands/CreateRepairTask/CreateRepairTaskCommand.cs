using Application.Features.RepairTasks.Dtos;
using Domain.Common.Results;
using Domain.Features.RepairTasks;
using MediatR;

namespace Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public record CreateRepairTaskCommand(string? Name, decimal LaborCost,RepairTaskDurationInMinutes TaskDuration,List<CreatePartCommand> CreatePartCommands) : IRequest<Result<RepairTaskDto>>;
}