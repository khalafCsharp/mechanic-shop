using Application.Common.Interfaces;
using Application.Features.Customers.Dtos;
using Application.Features.Customers.Mappers;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Domain.Features.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<GetCustomerByIdQueryHandler> logger;
        public GetCustomerByIdQueryHandler(ILogger<GetCustomerByIdQueryHandler> logger, IAppDbContext context)
        {
            this.logger = logger;
            this.context = context;

        }
        public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            Customer? customer = await context.Customers.AsNoTracking().Include(c => c.Vehicles).FirstOrDefaultAsync(c => c.Id == request.CustomerId);

            if (customer is null)
            {
                logger.LogWarning("Customer with id {CustomerId} was not found", request.CustomerId);
                return Error.NotFound(code: "Customer_NotFound",
                description: $"Customer with id '{request.CustomerId}' was not found");
            }
            return customer.ToDto();
        }
    }
}