using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.WorkOrders.Dtos;
using Domain.Common.Results;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Enums;

namespace Application.Features.WorkOrders.Queries.GetWorkOrders
{
    public record GetWorkOrdersQuery(int Page,
    int PageSize,
    string? SearchTerm,
    string SortColumn = "createdAt",
    string SortDirection = "desc",
    WorkOrderState? State = null,
    Guid? VehicleId = null,
    Guid? LaborId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Spot? Spot = null) : ICachedQuery<Result<PaginatedList<WorkOrderSummaryDto>>>
    {
        public string CacheKey => $"work-orders:p={Page}:ps={PageSize}" +
        $":q={SearchTerm ?? "-"}" +
        $":sort={SortColumn}:{SortDirection}" +
        $":state={State?.ToString() ?? "-"}" +
        $":veh={VehicleId?.ToString() ?? "-"}" +
        $":lab={LaborId?.ToString() ?? "-"}" +
        $":sd={StartDate?.ToString("yyyyMMdd") ?? "-"}" +
        $":edfrom={EndDate?.ToString("yyyyMMdd") ?? "-"}" +
        $":spot={Spot?.ToString() ?? "-"}";

        public string[] Tags => ["work-orders"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}