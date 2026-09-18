using Domain.Features.WorkOrders.Billing;

namespace Application.Common.Interfaces
{
   public interface IInvoicePdfGenerator
{
    byte[] Generate(Invoice invoice);
}
}