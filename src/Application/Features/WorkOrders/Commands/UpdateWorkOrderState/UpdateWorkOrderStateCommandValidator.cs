using Application.Features.WorkOrders.Commands.UpdateWorkOrderState;
using FluentValidation;

public class UpdateWorkOrderStateCommandValidator : AbstractValidator<UpdateWorkOrderStateCommand>
{
    public UpdateWorkOrderStateCommandValidator()
    {
        RuleFor(command => command.WorkOrderId)
     .NotEmpty()
     .WithErrorCode("WorkOrderId_Required")
     .WithMessage("WorkOrderId is required.");

        RuleFor(command => command.NewState)
          .IsInEnum()
          .WithErrorCode("WorkOrderStatus_Invalid")
          .WithMessage("Status must be a valid WorkOrderStatus value.");
    }
}