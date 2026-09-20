using FluentValidation;

namespace Application.Features.Billing.Queries.GetInvoiceById
{
    public class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
    {
        public GetInvoiceByIdQueryValidator()
        {
            RuleFor(request => request.InvoiceId)
           .NotEmpty()
           .WithErrorCode("InvoiceId_Is_Required")
           .WithMessage("InvoiceId is required.");
        }
    }
}