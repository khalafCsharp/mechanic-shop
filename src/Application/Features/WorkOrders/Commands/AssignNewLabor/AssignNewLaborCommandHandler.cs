using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Domain.Features.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Commands.AssignNewLabor
{
    public class AssignNewLaborCommandHandler : IRequestHandler<AssignNewLaborCommand, Result<Updated>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<AssignNewLaborCommandHandler> logger;
        private readonly HybridCache cache;
        private readonly IWorkOrderPolicy workOrderPolicy;

        public AssignNewLaborCommandHandler(IAppDbContext context, ILogger<AssignNewLaborCommandHandler> logger, HybridCache cache, IWorkOrderPolicy workOrderPolicy)
        {
            this.cache = cache;
            this.workOrderPolicy = workOrderPolicy;
            this.context = context;
            this.logger = logger;
        }
        public async Task<Result<Updated>> Handle(AssignNewLaborCommand request, CancellationToken cancellationToken)
        {
            WorkOrder? workOrderToBeUpdated = await context.WorkOrders
           .FirstOrDefaultAsync(a => a.Id == request.WorkOrderId, cancellationToken);

            if (workOrderToBeUpdated is null)
            {
                logger.LogError("WorkOrder with id: {workOrderId} does not exist.", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }

            var laborToBeAssigned = await context.Employees.FindAsync([request.LaborId], cancellationToken);

            if (laborToBeAssigned is null)
            {
                logger.LogError("Invalid LaborId: {laborId}", request.LaborId);
                return ApplicationErrors.LaborNotFound;
            }

            if (await workOrderPolicy.IsLaborOccupied(request.LaborId, request.WorkOrderId, workOrderToBeUpdated.StartAtUtc, workOrderToBeUpdated.EndAtUtc))
            {
                logger.LogError("Labor with id: {laborId} is already occupied during the requested time.", workOrderToBeUpdated.LaborId);
                return ApplicationErrors.LaborOccupied;
            }

            var updateLaborResult = workOrderToBeUpdated.UpdateLabor(request.LaborId);

            if (updateLaborResult.IsError)
            {
                foreach (Error error in updateLaborResult.Errors)
                {
                    logger.LogError("[LaborUpdate] {ErrorCode}: {ErrorDescription}", error.Code, error.Description);
                }

                return updateLaborResult.Errors;
            }

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("work-order", cancellationToken);

            return Result.Updated;
        }
    }
}