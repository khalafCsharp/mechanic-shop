using FluentValidation;

namespace Application.Features.WorkOrders.Commands.AssignNewLabor
{
    public class AssignNewLaborCommandValidator : AbstractValidator<AssignNewLaborCommand>
    {
        public AssignNewLaborCommandValidator()
        {
            RuleFor(command => command.WorkOrderId)
       .NotEmpty()
       .WithErrorCode("WorkOrderId_Required")
       .WithMessage("WorkOrderId is required.");

            RuleFor(command => command.LaborId)
               .NotEmpty()
               .WithErrorCode("LaborId_Required")
               .WithMessage("LaborId is required.");
        }
    }
}