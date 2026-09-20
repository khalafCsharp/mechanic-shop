using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Features.WorkOrders.Dtos;
using Application.Features.WorkOrders.Mappers;
using Domain.Common.Results;
using Domain.Features.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<GetWorkOrderByIdQueryHandler> logger;
        public GetWorkOrderByIdQueryHandler(IAppDbContext context, ILogger<GetWorkOrderByIdQueryHandler> logger)
        {
            this.logger = logger;
            this.context = context;

        }
        public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
        {
            WorkOrder? workOrder = await context.WorkOrders.AsNoTracking().Include(w => w.RepairTasks).ThenInclude(rt => rt.Parts).Include(w => w.Vehicle).ThenInclude(v => v.Customer).Include(w => w.Labor).Include(w => w.Invoice).FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);

            if (workOrder is null)
            {
                logger.LogWarning("work order was not found for id: {workOrderId}", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }
            return workOrder.ToDto();
        }
    }
}