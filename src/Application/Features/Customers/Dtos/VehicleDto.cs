namespace Application.Features.Customers.Dtos
{
    public record VehicleDto(Guid VehicleId, string Make, string Model, int Year, string LicensePlate);
}