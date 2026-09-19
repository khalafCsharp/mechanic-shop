using FluentValidation;

namespace Application.Features.Identity.Queries.GenerateTokens
{
    public class GenerateTokenQueryValidator : AbstractValidator<GenerateTokenQuery>
    {
        public GenerateTokenQueryValidator()
        {
            RuleFor(query =>query.Email).EmailAddress().WithMessage("emial must be valid").MaximumLength(100);

            RuleFor(query =>query.Password).NotEmpty().WithMessage("password is required");
        }
    }
}