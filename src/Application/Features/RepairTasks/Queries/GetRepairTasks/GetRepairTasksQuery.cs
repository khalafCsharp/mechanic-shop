using Application.Common.Interfaces;
using Application.Features.RepairTasks.Dtos;
using Domain.Common.Results;

namespace Application.Features.RepairTasks.Queries.GetRepairTasks
{
    public record GetRepairTasksQuery() : ICachedQuery<Result<List<RepairTaskDto>>>
    {
        public string CacheKey => "repair-tasks";

        public string[] Tags => ["repair-tasks"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}