using Domain.Common.Results;
using Domain.Features.RepairTasks;
using Domain.Features.RepairTasks.Parts;

namespace MechanicShop.Test.Common.RepairTasks
{

    public static class RepairTaskFactory
    {
        public static Result<RepairTask> CreateRepairTask(Guid? Id = null, string? Name = null, decimal? LaborCost = null, RepairTaskDurationInMinutes? DurationInMinutes = null, List<Part>? Parts = null)
        {
            return RepairTask.Create(
                Id ?? Guid.NewGuid(),
                Name ?? "Engine change",
                LaborCost ?? 200m,
                DurationInMinutes ?? RepairTaskDurationInMinutes.Min30,
                Parts ?? [PartFactory.CreatePart(Guid.NewGuid(), Name: "engine", Cost: 100m, Quantity: 1).Value]
            );
        }
    }
}