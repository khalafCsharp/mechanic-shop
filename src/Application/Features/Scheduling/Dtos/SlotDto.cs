using Application.Features.Labors.Dtos;
using Application.Features.RepairTasks.Dtos;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Enums;

namespace Application.Features.Scheduling.Dtos
{
    public class SlotDto
    {
        public Guid? WorkOrderId { get; set; }
        public Spot Spot { get; set; }
        public DateTimeOffset StartAtUtc { get; set; }
        public DateTimeOffset EndAtUtc { get; set; }
        public string? Vehicle { get; set; }
        public LaborDto? Labor { get; set; }
        public bool IsOccupied { get; set; }
        public bool? IsAvailable { get; set; }
        public bool IsWorkOrderLocked { get; set; }
        public WorkOrderState? State { get; set; }
        public RepairTaskDto[]? RepairTasks { get; set; }
    }
}