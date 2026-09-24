using Domain.Common.Results;
using Domain.Features.RepairTasks;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Enums;
using MechanicShop.Test.Common.RepairTasks;

namespace MechanicShop.Test.Common.WorkOrders
{
    public static class WorkOrderFactory
    {
     public static Result<WorkOrder> CreateWorkOrder(
     Guid? Id = null,
     Guid? VehicleId = null,
     DateTimeOffset? StartAtUtc = null,
     DateTimeOffset? EndAtUtc = null,
     Guid? LaborId = null,
     Spot? spot = null,
     List<RepairTask>? RepairTasks = null)
        {
            return WorkOrder.Create(
                Id ?? Guid.NewGuid(),
                VehicleId ?? Guid.NewGuid(),
                StartAtUtc ?? DateTimeOffset.UtcNow,
                EndAtUtc ?? DateTimeOffset.UtcNow.AddHours(1),
                LaborId ?? Guid.NewGuid(),
                spot ?? Spot.A,
                RepairTasks ?? [RepairTaskFactory.CreateRepairTask().Value]);
        }
    }
}