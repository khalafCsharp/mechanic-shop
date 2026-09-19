using Domain.Features.RepairTasks;

namespace Application.Features.RepairTasks.Dtos
{
    public record RepairTaskDto(Guid RepairTaskId, string Name, RepairTaskDurationInMinutes TaskDurationInMinutes, decimal LaborCost, decimal TotalCost, List<PartDto> RepairTaskParts);
}