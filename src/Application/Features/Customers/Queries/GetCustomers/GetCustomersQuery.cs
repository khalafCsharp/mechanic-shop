using Application.Common.Interfaces;
using Application.Features.Customers.Dtos;
using Domain.Common.Results;

namespace Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQuery : ICachedQuery<Result<List<CustomerDto>>>
    {
        public string CacheKey => "customers";

        public string[] Tags => ["customers"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}