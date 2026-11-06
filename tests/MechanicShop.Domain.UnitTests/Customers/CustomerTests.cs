using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common.Results;
using Domain.Features.Customers;
using Domain.Features.Customers.Vehicles;
using MechanicShop.Test.Common.Customers;

namespace MechanicShop.Domain.UnitTests.Customers
{
    public class CustomerTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            string name = "Customer #1";
            string phoneNumber = "5555555555";
            string email = "customer01@localhost";
            List<Vehicle> vehicles = [VehicleFactory.CreateVehicle().Value];

            var result = CustomerFactory.CreateCustomer(Id: id, Name: name, PhoneNumber: phoneNumber, Email: email, Vehicles: vehicles);

            Assert.True(result.IsSuccess);

            Assert.IsType<Customer>(result.Value);
            Assert.NotNull(result.Value);
            Assert.Equal(id, result.Value.Id);
            Assert.Equal(name, result.Value.Name);
            Assert.Equal(phoneNumber, result.Value.PhoneNumber);
            Assert.Equal(email, result.Value.Email);
            Assert.Single(result.Value.Vehicles);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ShouldFail(string? invalidName)
        {
            var result = CustomerFactory.CreateCustomer(Name: invalidName);
            Assert.True(result.IsError);
        }

        [Theory]
        [InlineData("")] //  3 > value < 16
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("12345678910111213")]
        public void CreateCustomer_ShouldFail_WhenPhoneInvalid(string? invalidPhone)
        {
            var result = CustomerFactory.CreateCustomer(PhoneNumber: invalidPhone);
            Assert.True(result.IsError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithEmptyEmail_ShouldFail(string? value)
        {
            var result = CustomerFactory.CreateCustomer(Email: value);
            Assert.True(result.IsError);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("abc1.@")]
        [InlineData("abc1..@")]
        public void Create_WithInvalidEmail_ShouldFail(string? value)
        {
            var result = CustomerFactory.CreateCustomer(Email: value);
            Assert.True(result.IsError);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var customer = CustomerFactory.CreateCustomer().Value;

            var result = customer.Update("new Name", "updatedme@email.com", "9898073");

            Assert.True(result.IsSuccess);
            Assert.Equal(Result.Updated, result.Value);
        }

        [Fact]
        public void Update_WithInvalidName_ShouldFail()
        {
            var customer = CustomerFactory.CreateCustomer().Value;

            var result = customer.Update(string.Empty, "newEmail@localhost", "12331232");

            Assert.True(result.IsError);
        }

        [Fact]
        public void Update_WithInvalidPhoneNumber_ShouldFail()
        {
            var customer = CustomerFactory.CreateCustomer().Value;

            var result = customer.Update("updated name", "newEmail@localhost", string.Empty);

            Assert.True(result.IsError);
        }

        [Fact]
        public void Update_WithEmptyEmail_ShouldFail()
        {
            var customer = CustomerFactory.CreateCustomer().Value;

            var result = customer.Update("update me", string.Empty, "12391232");

            Assert.True(result.IsError);
        }

        [Fact]
        public void UpdateCustomerVehicles_WithValidData_ShouldSucceed()
        {
            // Given
            var existingVehicle = VehicleFactory.CreateVehicle(Year: 2020).Value;
            var customer = CustomerFactory.CreateCustomer(Name: "khalaf", Vehicles: [existingVehicle]).Value;

            var updatedVehicle = VehicleFactory.CreateVehicle(Id: existingVehicle.Id, Year: 2022).Value;
            var newVehicle = VehicleFactory.CreateVehicle(Year: 2025).Value;

            var result = customer.UpdateCustomerVehicles([updatedVehicle, newVehicle]);

            Assert.True(result.IsSuccess);
            Assert.Equal(2,customer.Vehicles.Count());

            Assert.Equal(Result.Updated,result.Value);

        }
        [Fact]
        public void UpdateCustomerVehicles_ShouldRemoveVehiclesThatNotInTheIncomingList()
        {
            var v1 = VehicleFactory.CreateVehicle().Value;
            var v2 = VehicleFactory.CreateVehicle().Value;
            var customer = CustomerFactory.CreateCustomer(Vehicles: [v1, v2]).Value;

            var incoming = VehicleFactory.CreateVehicle(Id: v2.Id).Value;

            var result = customer.UpdateCustomerVehicles([incoming]);

            Assert.Equal(Result.Updated, result.Value);
            Assert.True(result.IsSuccess);
            Assert.Single(customer.Vehicles);//must contain only v2
            Assert.Equal(v2.Id, customer.Vehicles.Single().Id);
        }
    }
}