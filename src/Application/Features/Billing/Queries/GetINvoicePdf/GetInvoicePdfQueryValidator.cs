using FluentValidation;

namespace Application.Features.Billing.Queries.GetINvoicePdf
{
    public class GetInvoicePdfQueryValidator: AbstractValidator<GetInvoicePdfQuery>
    {
        public GetInvoicePdfQueryValidator()
        {
            RuleFor(request => request.InvoiceId)
            .NotEmpty()
            .WithErrorCode("InvoiceId_Is_Required")
            .WithMessage("InvoiceId is required.");
        }
    }
}