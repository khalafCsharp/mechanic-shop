using FluentValidation;

namespace Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
    {
        public GetCustomerByIdQueryValidator()
        {
            RuleFor(request => request.CustomerId).NotEmpty().WithErrorCode("CustomerId_Is_Required").WithMessage("CustomerId is required");
        }
    }
}