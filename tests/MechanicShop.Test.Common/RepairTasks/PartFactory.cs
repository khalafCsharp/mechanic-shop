using Domain.Common.Results;
using Domain.Features.RepairTasks.Parts;

namespace MechanicShop.Test.Common.RepairTasks
{
    public static class PartFactory
    {
        public static Result<Part> CreatePart(Guid? Id=null,string? Name=null, decimal? Cost = null, int? Quantity=null)
        {
            return Part.Create(
                Id ?? Guid.NewGuid(),
                Name ?? "engine",
                Cost ?? 500m,
                Quantity ?? 1
            );

        }
    }
}