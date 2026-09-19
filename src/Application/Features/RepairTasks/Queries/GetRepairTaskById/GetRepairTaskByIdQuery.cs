using Application.Common.Interfaces;
using Application.Features.RepairTasks.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.RepairTasks.Queries.GetRepairTaskById
{
    public record GetRepairTaskByIdQuery(Guid RepairTaskId) : ICachedQuery<Result<RepairTaskDto>>
    {
        public string CacheKey => $"repair-task_{RepairTaskId}";

        public string[] Tags => ["repair-tasks"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }

}