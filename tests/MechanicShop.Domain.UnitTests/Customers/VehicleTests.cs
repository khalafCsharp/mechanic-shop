using Domain.Features.Customers.Vehicles;
using MechanicShop.Test.Common.Customers;

namespace MechanicShop.Domain.UnitTests.Customers
{
    public class VehicleTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Given
            Guid id = Guid.NewGuid();
            string make = "Toyota";
            string model = "corolla";
            int year = 2000;
            string licensePlate = "KING1";


            // When
            var result = VehicleFactory.CreateVehicle(id, make, model, year, licensePlate);
            // Then

            Assert.True(result.IsSuccess);
            Assert.Equal(id, result.Value.Id);
            Assert.Equal(make, result.Value.Make);
            Assert.Equal(model, result.Value.Model);
            Assert.Equal(year, result.Value.Year);
            Assert.Equal(licensePlate, result.Value.LicensePlate);

        }
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidMake_ShouldFail(string make)
        {
            // When
            var result = VehicleFactory.CreateVehicle(Make: make);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(result.TopError.Code, VehicleErrors.MakeRequired.Code);
            Assert.Equal(result.TopError.Description, VehicleErrors.MakeRequired.Description);
        }
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidModel_ShouldFail(string model)
        {
            // When
            var result = VehicleFactory.CreateVehicle(Model: model);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(result.TopError.Code, VehicleErrors.ModelRequired.Code);
            Assert.Equal(result.TopError.Description, VehicleErrors.ModelRequired.Description);
        }
        [Theory]
        [InlineData(1000)]
        [InlineData(3000)]
        public void Create_WithInvalidYear_ShouldFail(int year)
        {
            // When
            var result = VehicleFactory.CreateVehicle(Year: year);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(result.TopError.Code, VehicleErrors.YearInvalid.Code);
            Assert.Equal(result.TopError.Description, VehicleErrors.YearInvalid.Description);
        }
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidLicensePlate_ShouldFail(string licensePlate)
        {
            // When
            var result = VehicleFactory.CreateVehicle(LicensePlate: licensePlate);
            // Then
            Assert.True(result.IsError);
            Assert.Equal(result.TopError.Code, VehicleErrors.LicensePlateRequired.Code);
            Assert.Equal(result.TopError.Description, VehicleErrors.LicensePlateRequired.Description);
        }



        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {

            // Given
            var vehicle = VehicleFactory.CreateVehicle().Value;
            // When
            var result = vehicle.Update("honda", "cc", 2020, "HONDACC");
            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal("honda", vehicle.Make);
            Assert.Equal("cc", vehicle.Model);
            Assert.Equal(2020, vehicle.Year);
            Assert.Equal("HONDACC", vehicle.LicensePlate);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidMake_ShouldFail(string make)
        {
            var vehicle = VehicleFactory.CreateVehicle().Value;

            //when
            var result = vehicle.Update(make, "cc", 2020, "HONDACC");

            //then
            Assert.True(result.IsError);
        }
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidModel_ShouldFail(string model)
        {
            var vehicle = VehicleFactory.CreateVehicle().Value;

            //when
            var result = vehicle.Update("honda", model, 2020, "HONDACC");

            //then
            Assert.True(result.IsError);
        }
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidLicensePlate_ShouldFail(string licensePlate)
        {
            var vehicle = VehicleFactory.CreateVehicle().Value;

            //when
            var result = vehicle.Update("honda", "cc", 2020, licensePlate);

            //then
            Assert.True(result.IsError);
        }
        [Theory]
        [InlineData(47)]
        [InlineData(2200)]
        public void Update_WithInvalidYear_ShouldFail(int year)
        {
            var vehicle = VehicleFactory.CreateVehicle().Value;

            //when
            var result = vehicle.Update("honda", "cc", year, "HHHHOOOO");

            //then
            Assert.True(result.IsError);
        }

        [Fact]
        public void VehicleInfo_WithValidData_VehicleInfoShouldMatch()
        {
            //when
            var vehicle = VehicleFactory.CreateVehicle(Make: "Honda", Model: "CC", Year: 2020).Value;

            // Then
            Assert.Equal($"{vehicle.Make} | {vehicle.Model} | {vehicle.Year}", vehicle.VehicleInfo);
        }
    }
}