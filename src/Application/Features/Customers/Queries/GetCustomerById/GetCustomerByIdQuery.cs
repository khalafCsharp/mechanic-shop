using Application.Common.Interfaces;
using Application.Features.Customers.Dtos;
using Domain.Common.Results;

namespace Application.Features.Customers.Queries.GetCustomerById
{
    public record GetCustomerByIdQuery(Guid CustomerId) : ICachedQuery<Result<CustomerDto>>
    {
        public string CacheKey => $"customer_{CustomerId}";

        public string[] Tags => ["customer"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}