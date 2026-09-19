using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Features.Customers.Dtos;
using Application.Features.Customers.Mappers;
using Domain.Common.Results;
using Domain.Features.Customers;
using Domain.Features.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        private readonly HybridCache cache;
        private readonly ILogger<CreateCustomerCommandHandler> logger;
        private readonly IAppDbContext context;
        public CreateCustomerCommandHandler(HybridCache cache, ILogger<CreateCustomerCommandHandler> logger, IAppDbContext context)
        {
            this.context = context;
            this.logger = logger;
            this.cache = cache;

        }
        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            string email = request.Email.Trim().ToLower();

            bool isEmailAlreadyExists = await context.Customers.AnyAsync(c => c.Email!.ToLower() == email, cancellationToken);

            if (isEmailAlreadyExists)
            {
                logger.LogWarning("customer creation aborted, a customer with same email already exists.");
                return CustomerErrors.CustomerExists;
            }

            List<Vehicle> newCustomerVehicles = [];
            foreach (CreateVehicleCommand cvc in request.CreateVehicleCommands)
            {
                Result<Vehicle> createVehicleResult = Vehicle.Create(Guid.NewGuid(), cvc.Make, cvc.Model, cvc.Year, cvc.LicensePlate);

                if (createVehicleResult.IsError)
                {
                    return createVehicleResult.Errors;
                }
                newCustomerVehicles.Add(createVehicleResult.Value);
            }
            Result<Customer> createCustomerResult = Customer.Create(Guid.NewGuid(), request.Name.Trim(), request.PhoneNumber.Trim(), request.Email.Trim(), newCustomerVehicles);

            if (createCustomerResult.IsError)
            {
                return createCustomerResult.Errors;
            }
            context.Customers.Add(createCustomerResult.Value);
            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("customer", cancellationToken);

            logger.LogInformation("a new customer was created with id: {newCustomerId}", createCustomerResult.Value.Id);

            return createCustomerResult.Value.ToDto();

        }
    }
}