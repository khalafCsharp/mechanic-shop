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
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    DateTime? EndDateFrom = null,
    DateTime? EndDateTo = null,
    Spot? Spot = null) : ICachedQuery<Result<PaginatedList<WorkOrderSummaryDto>>>
    {
        public string CacheKey => $"work-orders:p={Page}:ps={PageSize}" +
        $":q={SearchTerm ?? "-"}" +
        $":sort={SortColumn}:{SortDirection}" +
        $":state={State?.ToString() ?? "-"}" +
        $":veh={VehicleId?.ToString() ?? "-"}" +
        $":lab={LaborId?.ToString() ?? "-"}" +
        $":sdfrom={StartDateFrom?.ToString("yyyyMMdd") ?? "-"}" +
        $":sdto={StartDateTo?.ToString("yyyyMMdd") ?? "-"}" +
        $":edfrom={EndDateFrom?.ToString("yyyyMMdd") ?? "-"}" +
        $":edto={EndDateTo?.ToString("yyyyMMdd") ?? "-"}" +
        $":spot={Spot?.ToString() ?? "-"}";

        public string[] Tags => ["work-orders"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}