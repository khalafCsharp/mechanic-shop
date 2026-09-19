using FluentValidation;

namespace Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleCommandValidator()
        {
            RuleFor(command => command.Make).NotEmpty().WithMessage("vehicle make should be included").MaximumLength(50);
            RuleFor(command => command.Model).NotEmpty().WithMessage("vehicle model should be included").MaximumLength(50);
            RuleFor(command => command.LicensePlate).NotEmpty().WithMessage("vehicle license plate number should be included").MaximumLength(10);

            RuleFor(command => command.Year).Must(year => year >= 1900 && year < DateTime.UtcNow.Year + 1);
        }
    }
}