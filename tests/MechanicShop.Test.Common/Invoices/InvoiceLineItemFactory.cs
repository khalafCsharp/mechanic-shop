using Domain.Common.Results;
using Domain.Features.WorkOrders.Billing;

namespace MechanicShop.Test.Common.Invoices
{

    public static class InvoiceLineItemFactory
    {
        public static Result<InvoiceLineItem> CreateInvoiceLineItem(Guid? Id = null, int? LineNumber = null, string? Description = null, int? Quantity = null, decimal? UnitPrice = null)
        {
            return InvoiceLineItem.Create(
                Id ?? Guid.NewGuid(),
                LineNumber ?? 2,
                Description ?? "second line in the invoice",
                Quantity ?? 4,
                UnitPrice ?? 100m
            );
        }
    }
}