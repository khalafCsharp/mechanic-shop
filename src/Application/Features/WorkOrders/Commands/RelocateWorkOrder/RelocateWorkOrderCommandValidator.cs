using FluentValidation;

namespace Application.Features.WorkOrders.Commands.RelocateWorkOrder
{
    public class RelocateWorkOrderCommandValidator : AbstractValidator<RelocateWorkOrderCommand>
    {
        public RelocateWorkOrderCommandValidator()
        {
            RuleFor(command => command.WorkOrderId)
                .NotEmpty();

            RuleFor(command => command.NewStartAt)
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage("New start time must be in the future.");

            RuleFor(command => command.NewSpot)
                .IsInEnum();
        }
    }
}