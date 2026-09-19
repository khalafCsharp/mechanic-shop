using FluentValidation;

namespace Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public class CreateRepairTaskCommandValidator : AbstractValidator<CreateRepairTaskCommand>
    {
        public CreateRepairTaskCommandValidator()
        {
            RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Name is required.")
      .MaximumLength(100);

            RuleFor(x => x.LaborCost)
                .GreaterThan(0).WithMessage("Labor cost must be greater than 0.");

            RuleFor(x => x.TaskDuration)
                .NotNull().WithMessage("Estimated duration is required.")
                .IsInEnum();

            RuleFor(x => x.CreatePartCommands)
                .NotNull().WithMessage("Parts list cannot be null.")
                .Must(p => p.Count > 0).WithMessage("At least one part is required.");

            RuleForEach(x => x.CreatePartCommands).SetValidator(new CreatePartCommandValidator());
        }
    }
}