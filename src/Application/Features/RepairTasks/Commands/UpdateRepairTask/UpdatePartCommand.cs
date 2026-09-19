namespace Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public record UpdatePartCommand(
    Guid? PartId,
    string Name,
    decimal Cost,
    int Quantity
);
}