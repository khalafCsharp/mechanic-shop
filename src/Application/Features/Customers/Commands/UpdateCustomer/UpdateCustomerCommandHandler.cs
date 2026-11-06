using System.ComponentModel;
using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Features.Customers;
using Domain.Features.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Updated>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<UpdateCustomerCommandHandler> logger;
        private readonly HybridCache cache;

        public UpdateCustomerCommandHandler(IAppDbContext context, ILogger<UpdateCustomerCommandHandler> logger, HybridCache cache)
        {
            this.cache = cache;
            this.logger = logger;
            this.context = context;
        }
        public async Task<Result<Updated>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            Customer? customerToBeUpdated = await context.Customers.Include(c => c.Vehicles).FirstOrDefaultAsync(c => c.Id == request.CustomerToBeUpdatedId, cancellationToken);

            if (customerToBeUpdated is null)
            {
                logger.LogWarning("customer with id: {customerId} not found for update", request.CustomerToBeUpdatedId);
                return ApplicationErrors.CustomerNotFound;
            }

            List<Vehicle> newVehicleList = [];
            foreach (UpdateVehicleCommand uvc in request.UpdateVehicleCommands)
            {
                Guid id = uvc.VehicleToBeUpdatedId ?? Guid.NewGuid();
                Result<Vehicle> createVehicleResult = Vehicle.Create(id, uvc.Make, uvc.Model, uvc.Year, uvc.LicensePlate);

                if (createVehicleResult.IsError)
                {
                    return createVehicleResult.Errors;
                }

                newVehicleList.Add(createVehicleResult.Value);
            }

            Result<Updated> updateCustomerInfoResult = customerToBeUpdated.Update(request.Name, request.Email, request.PhoneNumber);
            if (updateCustomerInfoResult.IsError)
            {
                return updateCustomerInfoResult.Errors;
            }

            Result<Updated> updateCustomerVehiclesInfoResult = customerToBeUpdated.UpdateCustomerVehicles(newVehicleList);

            if (updateCustomerVehiclesInfoResult.IsError)
            {
                return updateCustomerVehiclesInfoResult.Errors;
            }

            await context.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("customers", cancellationToken);

            return Result.Updated;
        }
    }
}