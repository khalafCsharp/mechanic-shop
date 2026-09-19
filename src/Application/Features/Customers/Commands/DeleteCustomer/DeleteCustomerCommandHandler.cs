using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Common.Results;
using Domain.Features.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<Deleted>>
    {
        private readonly ILogger<DeleteCustomerCommandHandler> logger;
        private readonly IAppDbContext context;
        private readonly HybridCache cache;

        public DeleteCustomerCommandHandler(ILogger<DeleteCustomerCommandHandler> logger,
        IAppDbContext context,
        HybridCache cache)
        {
            this.logger = logger;
            this.context = context;
            this.cache = cache;
        }
        public async Task<Result<Deleted>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            Customer? customerToBeDeleted = await context.Customers.FindAsync([request.CustomerId], cancellationToken);

            if (customerToBeDeleted is null)
            {
                logger.LogWarning("customer with id: {customerId} was not found for deletion", request.CustomerId);
                return ApplicationErrors.CustomerNotFound;
            }

            bool IsCustomerToBeDeletedHasAssociatedWorkOrders = await context.WorkOrders.Include(w => w.Vehicle).Where(w => w.Vehicle != null).AnyAsync(w => w.Vehicle!.CustomerId == request.CustomerId, cancellationToken);

            if (IsCustomerToBeDeletedHasAssociatedWorkOrders)
            {
                logger.LogWarning("customer with id: {customerId} can not be deleted because they have workorders", request.CustomerId);
                return CustomerErrors.CannotDeleteCustomerWithWorkOrders;
            }

            context.Customers.Remove(customerToBeDeleted);
            await context.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("customer", cancellationToken);

            logger.LogInformation("Customer {CustomerId} deleted successfully.", request.CustomerId);

            return Result.Deleted;
        }
    }
}