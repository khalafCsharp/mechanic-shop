using FluentValidation;

namespace Application.Features.Billing.Commands.SettleInvoice
{
    public class SettleInvoiceCommandValidator : AbstractValidator<SettleInvoiceCommand>
    {
        public SettleInvoiceCommandValidator()
        {
            RuleFor(request => request.InvoiceId)
                .NotEmpty()
                .WithErrorCode("InvoiceId_Is_Required")
                .WithMessage("InvoiceId is required.");
        }
    }
}