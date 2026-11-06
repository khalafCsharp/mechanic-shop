using Domain.Features.Employees;
using MechanicShop.Test.Common.Employees;

namespace MechanicShop.Domain.UnitTests.Employees
{
    public class EmmployeeTest
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Given
            Guid id = Guid.NewGuid();
            string firstName = "khalaf";
            string lastName = "ali";

            // When
            var result = EmployeeFactory.CreateLabor(id, firstName, lastName);
            // Then
            Assert.True(result.IsSuccess);
            Assert.Equal(id, result.Value.Id);
            Assert.Equal(firstName, result.Value.FirstName);
            Assert.Equal(lastName, result.Value.LastName);
            Assert.Equal($"{firstName} {lastName}", result.Value.FullName);
            Assert.Equal(Role.Labor, result.Value.Role);
        }

        [Fact]
        public void Create_WithEmptyFistName_ShouldFail()
        {
            // Given
            Guid id = Guid.NewGuid();
            string firstName = " ";
            string lastName = "ali";

            // When
            var result = EmployeeFactory.CreateLabor(id, firstName, lastName);

            // Then
            Assert.True(result.IsError);
            Assert.Equal(EmployeeErrors.FirstNameRequired.Code, result.TopError.Code);
            Assert.Equal(EmployeeErrors.FirstNameRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyLastName_ShouldFail()
        {
            // Given
            Guid id = Guid.NewGuid();
            string firstName = "Khalaf";
            string lastName = "  ";

            // When
            var result = EmployeeFactory.CreateLabor(id, firstName, lastName);

            // Then
            Assert.True(result.IsError);
            Assert.Equal(EmployeeErrors.LastNameRequired.Code, result.TopError.Code);
            Assert.Equal(EmployeeErrors.LastNameRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithInvalidRole_ShouldFail()
        {
            // Given
            Guid id = Guid.NewGuid();
            string firstName = "Khalaf";
            string lastName = "  ";

            // When
            var result = Employee.Create(id, firstName, lastName, (Role)4);

            // Then
            Assert.True(result.IsError);
            Assert.Equal(EmployeeErrors.RoleInvalid.Code, result.TopError.Code);
            Assert.Equal(EmployeeErrors.RoleInvalid.Description, result.TopError.Description);
        }
    }
}