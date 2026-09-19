using FluentValidation;

namespace Application.Features.RepairTasks.Queries.GetRepairTaskById
{
    public class GetRepairTaskByIdQueryValidator : AbstractValidator<GetRepairTaskByIdQuery>
    {
        public GetRepairTaskByIdQueryValidator()
        {
            RuleFor(query => query.RepairTaskId).NotEmpty().WithMessage("repair task id is required");
        }
    }
}