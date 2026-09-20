using Application.Common.Interfaces;
using Application.Features.Dashboard.Dtos;
using Domain.Common.Results;
using Domain.Features.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Dashboard.Queries.GetWorkOrderStats
{
    public class GetWorkOrderStatsQueryHandler : IRequestHandler<GetWorkOrderStatsQuery, Result<DailyWorkOrderStatsDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<GetWorkOrderStatsQueryHandler> logger;

        public GetWorkOrderStatsQueryHandler(IAppDbContext context, ILogger<GetWorkOrderStatsQueryHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<Result<DailyWorkOrderStatsDto>> Handle(GetWorkOrderStatsQuery request, CancellationToken cancellationToken)
        {
            var start = request.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = request.Date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            var query = context.WorkOrders
                .Include(w => w.Vehicle)
                .Include(w => w.RepairTasks).ThenInclude(rt => rt.Parts)
                .Include(w => w.Invoice)
                .Where(w => w.StartAtUtc >= start && w.StartAtUtc < end);

            var total = await query.CountAsync(cancellationToken);

            if (total == 0)
            {
                return new DailyWorkOrderStatsDto
                {
                    StatsDate = request.Date,
                    TotalWorkOrders = 0,
                    ScheduledWorkOrders = 0,
                    InProgressWorkOrders = 0,
                    CompletedWorkOrders = 0,
                    CancelledWorkOrders = 0,
                    TotalRevenue = 0,
                    TotalPartsCost = 0,
                    TotalLaborCost = 0,
                    UniqueVehicles = 0,
                    UniqueCustomers = 0
                };
            }
            var stats = await query.ToListAsync(cancellationToken);

            var totalRevenue = stats.Sum(w => w.Invoice?.Total ?? 0);
            var totalPartCost = stats.Where(w => w.Invoice != null).Sum(w => w.TotalPartsCost ?? 0);
            var totalLaborCost = stats.Where(w => w.Invoice != null).Sum(w => w.TotalLaborCost ?? 0);
            var uniqueVehicles = stats.Select(w => w.VehicleId).Distinct().Count();
            var uniqueCustomers = stats.Select(w => w.Vehicle!.CustomerId).Distinct().Count();

            var netProfit = totalRevenue - totalPartCost - totalLaborCost;

            return new DailyWorkOrderStatsDto
            {
                StatsDate = request.Date,
                TotalWorkOrders = total,
                ScheduledWorkOrders = stats.Count(x => x.State == WorkOrderState.Scheduled),
                InProgressWorkOrders = stats.Count(x => x.State == WorkOrderState.InProgress),
                CompletedWorkOrders = stats.Count(x => x.State == WorkOrderState.Completed),
                CancelledWorkOrders = stats.Count(x => x.State == WorkOrderState.Cancelled),
                TotalRevenue = totalRevenue,
                TotalPartsCost = totalPartCost,
                TotalLaborCost = totalLaborCost,
                UniqueVehicles = uniqueVehicles,
                UniqueCustomers = uniqueCustomers,
                NetProfit = netProfit,
                ProfitMargin = totalRevenue > 0 ? netProfit / totalRevenue * 100 : 0,
                CompletionRate = total > 0 ? (decimal)stats.Count(x => x.State == WorkOrderState.Completed) / total * 100 : 0,
                AverageRevenuePerOrder = total > 0 ? totalRevenue / total : 0,
                OrdersPerVehicle = uniqueVehicles > 0 ? (decimal)total / uniqueVehicles : 0,
                PartsCostRatio = totalRevenue > 0 ? totalPartCost / totalRevenue * 100 : 0,
                LaborCostRatio = totalRevenue > 0 ? totalLaborCost / totalRevenue * 100 : 0,
                CancellationRate = total > 0 ? (decimal)stats.Count(x => x.State == WorkOrderState.Cancelled) / total * 100 : 0
            };

        }
    }
}