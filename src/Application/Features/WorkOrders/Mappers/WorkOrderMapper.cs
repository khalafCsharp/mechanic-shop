using Application.Features.Customers.Mappers;
using Application.Features.Labors.Dtos;
using Application.Features.RepairTasks.Mappers;
using Application.Features.WorkOrders.Dtos;
using Domain.Features.WorkOrders;

namespace Application.Features.WorkOrders.Mappers
{
    public static class WorkOrderMapper
    {
        public static WorkOrderDto ToDto(this WorkOrder workorder)
        {
            ArgumentNullException.ThrowIfNull(workorder);

            return new WorkOrderDto
            {
                WorkOrderId = workorder.Id,
                Spot = workorder.Spot,
                StartAtUtc = workorder.StartAtUtc,
                EndAtUtc = workorder.EndAtUtc,
                Labor = workorder.Labor is null ? null : new LaborDto
                (
                    LaborId : workorder.LaborId,
                    Name : $"{workorder.Labor.FirstName} {workorder.Labor.LastName}"
                ),
                RepairTasks = workorder.RepairTasks.ToDtos(),
                Vehicle = workorder.Vehicle is null ? null : workorder.Vehicle.ToDto(),
                State = workorder.State,
                TotalPartCost = workorder.RepairTasks.SelectMany(t => t.Parts).Sum(p => p.Cost * p.Quantity),
                TotalLaborCost = workorder.RepairTasks.Sum(p => p.LaborCost),
                TotalCost = workorder.RepairTasks.Sum(rt => rt.TotalCost),
                TotalDurationInMins = workorder.RepairTasks.Sum(rt => (int)rt.EstimatedDurationInMins),
                InvoiceId = workorder.Invoice?.Id,
                CreatedAt = workorder.CreatedAtUtc
            };
        }

        public static List<WorkOrderDto> ToDtos(this IEnumerable<WorkOrder> entities)
        {
            return [.. entities.Select(e => e.ToDto())];
        }

        public static WorkOrderSummaryDto ToListItemDto(this WorkOrder workorder)
        {
            ArgumentNullException.ThrowIfNull(workorder);

            return new WorkOrderSummaryDto
            {
                WorkOrderId = workorder.Id,
                Spot = workorder.Spot,
                StartAtUtc = workorder.StartAtUtc,
                EndAtUtc = workorder.EndAtUtc,
                Vehicle = workorder.Vehicle!.ToDto(),
                Labor = workorder.Labor is null ? null :
                    $"{workorder.Labor.FirstName} {workorder.Labor.LastName}",
                State = workorder.State,
                RepairTasks = workorder.RepairTasks.Select(rt => rt.Name).ToList()
            };
        }
    }
}