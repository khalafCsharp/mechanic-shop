namespace Application.Features.Billing.Dtos
{
    public record InvoiceLineItemDto(Guid InvoiceId, int LineNumber, string? Description, int Quantity, decimal UnitPrice, decimal LineTotal);
}