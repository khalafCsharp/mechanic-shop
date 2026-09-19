using FluentValidation;

namespace Application.Features.RepairTasks.Commands.DeleteRepairTask
{
    public class DeleteRepairTaskCommandValidator : AbstractValidator<DeleteRepairTaskCommand>
    {
        public DeleteRepairTaskCommandValidator()
        {
            RuleFor(command => command.RepairTaskToBeDeletedId).NotEmpty().WithMessage("provide id of a repair task which you want to be deleted");
        }
    }
}