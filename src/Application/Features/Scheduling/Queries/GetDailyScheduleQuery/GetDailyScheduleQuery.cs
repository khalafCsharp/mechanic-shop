namespace MechanicShop.Application.Features.Scheduling.Queries.GetDailyScheduleQuery
{
    using Domain.Common.Results;
    using global::Application.Common.Interfaces;
    using global::Application.Features.Scheduling.Dtos;

    public sealed record GetDailyScheduleQuery(
    TimeZoneInfo TimeZone,
    DateOnly ScheduleDate,
    Guid? LaborId = null) : ICachedQuery<Result<ScheduleDto>>
    {
        public string CacheKey => $"work-order:{this.ScheduleDate:yyyy-MM-dd}:labor={this.LaborId?.ToString() ?? "-"}";

        public string[] Tags => ["work-orders"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}
