using Application.Common.Interfaces;
using Application.Features.Billing.Dtos;
using Domain.Common.Results;

namespace Application.Features.Billing.Queries.GetInvoiceById
{
    public record GetInvoiceByIdQuery(Guid InvoiceId) : ICachedQuery<Result<InvoiceDto>>
    {
        public string CacheKey => $"invoice_{InvoiceId}";

        public string[] Tags => ["invoices"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}