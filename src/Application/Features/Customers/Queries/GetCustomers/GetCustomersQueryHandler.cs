using Application.Common.Interfaces;
using Application.Features.Customers.Dtos;
using Application.Features.Customers.Mappers;
using Domain.Common.Results;
using Domain.Features.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
    {
        private readonly IAppDbContext _context;
        public GetCustomersQueryHandler(IAppDbContext context)
        {
            this._context = context;
        }

        public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            List<Customer> customers = await _context.Customers.Include(c => c.Vehicles).AsNoTracking().ToListAsync(cancellationToken);

            return customers.ToDtos();
        }
    }
}