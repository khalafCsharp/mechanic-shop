using FluentValidation;

namespace Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(command => command.Name).NotEmpty().WithMessage("name is required").MaximumLength(50);
            RuleFor(command => command.Email).EmailAddress().WithMessage("not a valid email address").MaximumLength(100);

            RuleFor(command => command.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7–15 digits and may start with '+'.");

            RuleFor(command => command.CreateVehicleCommands).NotNull().WithMessage("vehicle commands must not be null").Must(v => v.Count > 0).WithMessage("must have at least one create vehicle command");

            RuleForEach(command => command.CreateVehicleCommands).SetValidator(new CreateVehicleCommandValidator());
        }
    }
}