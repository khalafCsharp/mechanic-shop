using FluentValidation;

namespace Application.Features.WorkOrders.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdQueryValidator : AbstractValidator<GetWorkOrderByIdQuery>
    {
        public GetWorkOrderByIdQueryValidator()
        {
            RuleFor(request => request.WorkOrderId)
            .NotEmpty()
            .WithErrorCode("WorkOrderId_Is_Required")
            .WithMessage("WorkOrderId is required.");
        }
    }
}