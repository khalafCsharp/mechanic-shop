using Application.Features.RepairTasks.Dtos;
using Domain.Features.RepairTasks;
using Domain.Features.RepairTasks.Parts;

namespace Application.Features.RepairTasks.Mappers
{
    public static class RepairTaskMapper
    {
        public static PartDto ToDto(this Part? part)
        {
            if (part is null)
            {
                throw new ArgumentNullException("tried to map part to partdto when part is null");
            }
            return new PartDto(part.Id, part.Name!, part.Cost, part.Quantity);
        }
        public static List<PartDto> ToDtos(this IEnumerable<Part> parts)
        {

            return [.. parts.Select(p => p.ToDto())];
        }
        public static List<RepairTaskDto> ToDtos(this IEnumerable<RepairTask> rts)
        {

            return [.. rts.Select(rt => rt.ToDto())];
        }
        public static RepairTaskDto ToDto(this RepairTask? rt)
        {
            if (rt is null)
            {
                throw new ArgumentNullException("tried to map repair task to repairtaskdto when it is null");
            }
            return new RepairTaskDto(rt.Id, rt.Name, rt.EstimatedDurationInMins, rt.LaborCost, rt.TotalCost, rt.Parts.ToList().ToDtos());
        }
    }
}