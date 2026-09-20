using Application.Common.Interfaces;
using Application.Features.Scheduling.Dtos;
using Domain.Common.Results;

namespace Application.Features.Scheduling.Queries.GetDailyScheduleQuery
{
    public record GetDailyScheduleQuery(TimeZoneInfo TimeZone, DateOnly ScheduleDate, Guid? LaborId=null) : ICachedQuery<Result<ScheduleDto>>
    {
        public string CacheKey => $"work-order:{ScheduleDate:yyyy-MM-dd}:labor={LaborId?.ToString() ?? "-"}";

        public string[] Tags => ["work-orders"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}