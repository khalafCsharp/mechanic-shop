using FluentValidation;

namespace Application.Features.WorkOrders.Commands.DeleteWorkOrder
{
    public class DeleteWorkOrderCommandValidator : AbstractValidator<DeleteWorkOrderCommand>
    {
        public DeleteWorkOrderCommandValidator()
        {
            RuleFor(command => command.WorkOrderId)
               .NotEmpty()
               .WithErrorCode("WorkOrderId_Required")
               .WithMessage("WorkOrderId is required.");
        }
    }
}