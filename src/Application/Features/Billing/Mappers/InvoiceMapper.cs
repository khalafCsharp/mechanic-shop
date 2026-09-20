using Application.Features.Billing.Dtos;
using Application.Features.Customers.Mappers;
using Domain.Features.WorkOrders.Billing;

namespace Application.Features.Billing.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceLineItemDto ToDto(this InvoiceLineItem item)
        {
            return new InvoiceLineItemDto(InvoiceId: item.InvoiceId,
                LineNumber: item.LineNumber,
                Description: item.Description,
                Quantity: item.Quantity,
                UnitPrice: item.UnitPrice,
                LineTotal: item.LineTotal);
        }
        public static List<InvoiceLineItemDto> ToDtos(this IEnumerable<InvoiceLineItem> invoiceLines)
        {
            return [.. invoiceLines.Select(e => e.ToDto())];
        }
        public static InvoiceDto ToDto(this Invoice invoice)
        {
            ArgumentNullException.ThrowIfNull(invoice);

            return new InvoiceDto
            {
                InvoiceId = invoice.Id,
                WorkOrderId = invoice.WorkOrderId,
                Customer = invoice.WorkOrder!.Vehicle!.Customer.ToDto(),
                Vehicle = invoice.WorkOrder.Vehicle.ToDto(),
                IssuedAtUtc = invoice.IssuedAtUtc,
                Subtotal = invoice.Subtotal,
                TaxAmount = invoice.TaxAmount,
                DiscountAmount = invoice.DiscountAmount,
                Total = invoice.Total,
                PaymentStatus = invoice.State.ToString(),
                Items = invoice.LineItems.Select(li => li.ToDto()).ToList()
            };
        }

        public static List<InvoiceDto> ToDtos(this IEnumerable<Invoice> invoices)
        {
            return [.. invoices.Select(e => e.ToDto())];
        }

    }
}