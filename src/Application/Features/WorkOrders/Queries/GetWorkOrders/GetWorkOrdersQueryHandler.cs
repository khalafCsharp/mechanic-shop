namespace Application.Features.WorkOrders.Queries.GetWorkOrders
{
    using Application.Common.Interfaces;
    using Application.Common.Models;
    using Application.Features.Customers.Mappers;
    using Application.Features.WorkOrders.Dtos;
    using Domain.Common.Results;
    using Domain.Features.WorkOrders;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Hybrid;

/// <summary>
/// this class provides handler in which a mediatr command will be handled and return proper result.
/// </summary>
    public class GetWorkOrdersQueryHandler : IRequestHandler<GetWorkOrdersQuery, Result<PaginatedList<WorkOrderSummaryDto>>>
    {
        private readonly IAppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkOrdersQueryHandler"/> class.
        /// Initializes a new instance of <see cref="GetWorkOrdersQueryHandler"/> class.
        /// </summary>
        /// <param name="context"> an abstraction of efcore's DbContext.</param>
        public GetWorkOrdersQueryHandler(IAppDbContext context)
        { 
            this.context = context;
        }

/// <summary>
/// this method handles returning a paginated list of summary of workorders.
/// </summary>
/// <param name="request"> the request that will be handled in this handler method.</param>
/// <param name="cancellationToken"> cancellation token.</param>
/// <returns> returns a list of paginated workorder summary dto.</returns>
        public async Task<Result<PaginatedList<WorkOrderSummaryDto>>> Handle(GetWorkOrdersQuery request, CancellationToken cancellationToken)
        {
            var workOrdersQuery = this.context.WorkOrders.AsNoTracking()
          .Include(wo => wo.Vehicle!)
              .ThenInclude(v => v.Customer)
          .Include(wo => wo.Labor)
          .Include(wo => wo.RepairTasks)
              .ThenInclude(rt => rt.Parts)
          .Include(a => a.Invoice)
          .AsQueryable();

            // apply filters
            workOrdersQuery = this.ApplyFilters(workOrdersQuery, request);

            // filter by search term
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                workOrdersQuery = this.ApplySearchTerm(workOrdersQuery, request.SearchTerm.Trim().ToLower());
            }

 // apply sorting
            workOrdersQuery = this.ApplySorting(workOrdersQuery, request.SortColumn, request.SortDirection);

            int workOrdersCount = await workOrdersQuery.CountAsync(cancellationToken: cancellationToken);

            var returnedWorkOrderSummaryList = await workOrdersQuery.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).Select(wo => new WorkOrderSummaryDto()
            {
                WorkOrderId = wo.Id,
                InvoiceId = wo.Invoice == null ? null : wo.Invoice.Id,
                Spot = wo.Spot,
                StartAtUtc = wo.StartAtUtc,
                EndAtUtc = wo.EndAtUtc,
                Vehicle = wo.Vehicle!.ToDto(),
                Customer = wo.Vehicle!.Customer!.Name,
                Labor = wo.Labor != null
                        ? wo.Labor.FirstName + " " + wo.Labor.LastName
                        : null,
                State = wo.State,
                RepairTasks = wo.RepairTasks.Select(rt => rt.Name).ToList(),
            }).ToListAsync(cancellationToken);

            return new PaginatedList<WorkOrderSummaryDto>
            {
                Items = returnedWorkOrderSummaryList,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalItems = workOrdersCount,
                TotalPages = (int)Math.Ceiling(workOrdersCount / (double)request.PageSize),
            };
        }

        private IQueryable<WorkOrder> ApplyFilters(IQueryable<WorkOrder> query, GetWorkOrdersQuery request)
        {
            if (request.State.HasValue)
            {
                query = query.Where(wo => wo.State == request.State);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(wo => wo.StartAtUtc >= request.StartDate);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(wo => wo.EndAtUtc <= request.EndDate.Value);
            }

            return query;
        }

        private IQueryable<WorkOrder> ApplySearchTerm(IQueryable<WorkOrder> query, string term)
        {
            var searchedQuery = query.Where(wo => (wo.Vehicle != null && (wo.Vehicle.Make.ToLower().Contains(term) || wo.Vehicle.Model.ToLower().Contains(term) || wo.Vehicle.LicensePlate.ToLower().Contains(term)))
            || (wo.Labor != null && (
                wo.Labor.FirstName.ToLower().Contains(term) ||
                wo.Labor.LastName.ToLower().Contains(term) ||
                (wo.Labor.FirstName + " " + wo.Labor.LastName).ToLower().Contains(term)))
            || wo.RepairTasks.Any(rt =>
                rt.Name.ToLower().Contains(term)) ||
            wo.Id.ToString().ToLower().Contains(term));

            return searchedQuery;
        }

        private IQueryable<WorkOrder> ApplySorting(IQueryable<WorkOrder> query, string sortColumn, string sortDirection)
        {
            bool isDescending = sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase);

            IQueryable<WorkOrder> sortedQuery =
            sortColumn.ToLower() switch
            {
                "createdat" => isDescending ? query.OrderByDescending(wo => wo.CreatedAtUtc) : query.OrderBy(wo => wo.CreatedAtUtc),

                "updatedat" => isDescending ? query.OrderByDescending(wo => wo.LastModifiedAtUtc) : query.OrderBy(wo => wo.LastModifiedAtUtc),

                "startat" => isDescending ? query.OrderByDescending(wo => wo.StartAtUtc) : query.OrderBy(wo => wo.StartAtUtc),
                "endat" => isDescending ? query.OrderByDescending(wo => wo.EndAtUtc) : query.OrderBy(wo => wo.EndAtUtc),
                "state" => isDescending ? query.OrderByDescending(wo => wo.State) : query.OrderBy(wo => wo.State),
                "spot" => isDescending ? query.OrderByDescending(wo => wo.Spot) : query.OrderBy(wo => wo.Spot),

                "vehicleid" => isDescending ? query.OrderByDescending(wo => wo.VehicleId) : query.OrderBy(wo => wo.VehicleId),
                "laborid" => isDescending ? query.OrderByDescending(wo => wo.LaborId) : query.OrderBy(wo => wo.LaborId),
                _ => query.OrderByDescending(wo => wo.CreatedAtUtc),
            };
            return sortedQuery;
        }
    }
}
