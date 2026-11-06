using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common.Results;
using Domain.Features.WorkOrders.Billing;

namespace MechanicShop.Test.Common.Invoices
{
    public static class InvoiceFactory
    {
        public static Result<Invoice> CreateInvoice(Guid? Id = null, Guid? WorkOrderId = null, List<InvoiceLineItem>? Items = null, decimal? Discount = null,
        decimal? TaxAmount = null, TimeProvider? TimeProvider = null)
        {
            return Invoice.Create(
                Id ?? Guid.NewGuid(),
                WorkOrderId ?? Guid.NewGuid(),
                Items ?? [InvoiceLineItem.Create(invoiceId: Guid.NewGuid(), lineNumber: 3,
                 description: "changing front lights",
                  quantity: 2, unitPrice: 200m).Value],
                Discount ?? 0, TaxAmount ?? 0,
                TimeProvider ?? TimeProvider.System
            );
        }
    }
}