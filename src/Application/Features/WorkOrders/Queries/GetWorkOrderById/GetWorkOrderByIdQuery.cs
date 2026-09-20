using Application.Common.Interfaces;
using Application.Features.WorkOrders.Dtos;
using Domain.Common.Results;

namespace Application.Features.WorkOrders.Queries.GetWorkOrderById
{
    public record GetWorkOrderByIdQuery(Guid WorkOrderId) : ICachedQuery<Result<WorkOrderDto>>
    {
        public string CacheKey => $"work-order_{WorkOrderId}";

        public string[] Tags => ["work-orders"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}