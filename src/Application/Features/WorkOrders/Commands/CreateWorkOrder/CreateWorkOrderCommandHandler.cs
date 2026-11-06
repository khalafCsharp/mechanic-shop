using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Features.WorkOrders.Dtos;
using Application.Features.WorkOrders.Mappers;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Domain.Features.Customers.Vehicles;
using Domain.Features.Employees;
using Domain.Features.RepairTasks;
using Domain.Features.WorkOrders;
using Domain.Features.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.WorkOrders.Commands.CreateWorkOrder
{
    public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<CreateWorkOrderCommandHandler> logger;
        private readonly HybridCache cache;
        private readonly IWorkOrderPolicy workOrderPolicy;
        private readonly IMediator mediator;

        public CreateWorkOrderCommandHandler(IAppDbContext context, ILogger<CreateWorkOrderCommandHandler> logger, HybridCache cache, IWorkOrderPolicy workOrderPolicy, IMediator mediator)
        {
            this.cache = cache;
            this.workOrderPolicy = workOrderPolicy;
            this.mediator = mediator;
            this.context = context;
            this.logger = logger;
        }
        public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            List<RepairTask> repairTasks = await context.RepairTasks.Where(rt => request.RepairTaskIds.Contains(rt.Id)).ToListAsync(cancellationToken);

            if (repairTasks.Count != request.RepairTaskIds.Count)
            {
                Guid[] extraIdsSentByUser = request.RepairTaskIds.Except(repairTasks.Select(rt => rt.Id)).ToArray();

                logger.LogError("Some RepairTaskIds not found in database: {extraIds}", string.Join(", ", extraIdsSentByUser));

                return ApplicationErrors.RepairTaskNotFound;
            }

            TimeSpan workOrderEstimatedDuration = TimeSpan.FromMinutes(repairTasks.Sum(rt => ((int)rt.EstimatedDurationInMins)));

            DateTimeOffset endAtUtc = request.StartAtUtc.Add(workOrderEstimatedDuration);

            if (workOrderPolicy.IsOutsideOperatingHours(request.StartAtUtc, workOrderEstimatedDuration))
            {
                logger.LogError(" work order timing is invalid, {startTime} or {endTime} is outside the working time", request.StartAtUtc, endAtUtc);

                return ApplicationErrors.WorkOrderOutsideOperatingHour(request.StartAtUtc, endAtUtc);
            }

            var checkMinimumRequiredDurationForWorkOrdersResult = workOrderPolicy.ValidateMinimumAppointmentDurationRequiredInMinutes(request.StartAtUtc, endAtUtc);

            if (checkMinimumRequiredDurationForWorkOrdersResult.IsError)
            {
                logger.LogError("work order duration is shorter than required duration");
                return checkMinimumRequiredDurationForWorkOrdersResult.Errors;
            }
            var spotAvailabilityCheckResult = await workOrderPolicy.CheckSpotAvailabilityAsync(request.Spot, request.StartAtUtc, endAtUtc);

            if (spotAvailabilityCheckResult.IsError)
            {
                logger.LogError("spot is not available, spot: {spot}", request.Spot.ToString());

                return spotAvailabilityCheckResult.Errors;
            }

            Vehicle? vehicleToBeAppointedToWorkOrder = await this.context.Vehicles.Include(v => v.Customer).FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

            if (vehicleToBeAppointedToWorkOrder is null)
            {
                this.logger.LogError("vehicel with id: {vehicleId} not found", request.VehicleId);

                return ApplicationErrors.VehicleNotFound;
            }

            Employee? laborToBeAssignedToWorkOrder = await this.context.Employees.FindAsync([request.LaborId], cancellationToken);

            if (laborToBeAssignedToWorkOrder is null)
            {
                this.logger.LogError("labor with id: {laborId} does not exist", request.LaborId.ToString());

                return ApplicationErrors.LaborNotFound;
            }

            bool vehicleHasOnGoingWorkOrder = await this.context.WorkOrders.AnyAsync(wo => wo.VehicleId == request.VehicleId
            && wo.StartAtUtc < endAtUtc
            && wo.EndAtUtc > request.StartAtUtc, cancellationToken);

            if (vehicleHasOnGoingWorkOrder)
            {
                logger.LogError("Vehicle with id: {vehicleId} has an existing work order during same time", request.VehicleId);

                return Error.Conflict(code: "Vehicle_Overlapping_WorkOrder", "vehicle undergoes another repairment during same time");
            }
            bool isLaborOccupied = await context.WorkOrders
          .AnyAsync(
              a =>
              a.LaborId == request.LaborId &&
              a.StartAtUtc < endAtUtc &&
              a.EndAtUtc > request.StartAtUtc,
              cancellationToken);

            if (isLaborOccupied)
            {
                logger.LogError("Labor with Id '{laborId}' is already occupied during the requested time.", request.LaborId);
                return Error.Conflict(
                    code: "Labor_Occupied",
                    description: "labor is already occupied during the requested time.");
            }

            var createWorkOrderResult = WorkOrder.Create(
           Guid.NewGuid(),
           request.VehicleId,
           request.StartAtUtc,
           endAtUtc,
           request.LaborId!.Value,
           request.Spot,
           repairTasks);

            if (createWorkOrderResult.IsError)
            {
                logger.LogError("failed to create WorkOrder: {Error}", createWorkOrderResult.TopError.Description);

                return createWorkOrderResult.Errors;
            }

            var workOrder = createWorkOrderResult.Value;

            context.WorkOrders.Add(workOrder);

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await context.SaveChangesAsync(cancellationToken);

            workOrder.Vehicle = vehicleToBeAppointedToWorkOrder;
            workOrder.Labor = laborToBeAssignedToWorkOrder;

            logger.LogError("workOrder with id '{workOrderId}' created successfully.", workOrder.Id);

            
            
            await cache.RemoveByTagAsync("work-orders", cancellationToken);

            return workOrder.ToDto();
        }
    }
}