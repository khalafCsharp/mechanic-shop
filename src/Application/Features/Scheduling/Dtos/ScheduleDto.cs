namespace Application.Features.Scheduling.Dtos
{
    public class ScheduleDto
    {
        public DateOnly ScheduleDate { get; set; }
        public bool IsEndOfDay { get; set; }
        public List<SpotDto> Spots { get; set; } = [];
    }
}