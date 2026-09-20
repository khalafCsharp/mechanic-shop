using FluentValidation;

namespace Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks
{
    public class UpdateWorkOrderRepairTasksCommandValidator : AbstractValidator<UpdateWorkOrderRepairTasksCommand>
    {
        public UpdateWorkOrderRepairTasksCommandValidator()
        {
            RuleFor(command => command.WorkOrderId)
          .NotEmpty()
          .WithErrorCode("WorkOrderId_Required")
          .WithMessage("WorkOrderId is required.");

            RuleFor(command => command.RepairTaskIds)
              .NotEmpty()
              .WithErrorCode("RepairTasks_Required")
              .WithMessage("At least one repair task must be provided.");
        }
    }
}