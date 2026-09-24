using Domain.Common.Results;
using Domain.Features.Employees;

namespace MechanicShop.Test.Common.Employees
{
    public static class EmployeeFactory
    {
        public static Result<Employee> CreateLabor(Guid? Id=null, string? FirstName=null, string? LastName=null)
        {
            return Employee.Create(
                Id ?? Guid.NewGuid(),
                FirstName ?? "Labor #1 FirstName",
                FirstName ?? "Labor #1 LastName",
                Role.Labor
            );
        }
        public static Result<Employee> CreateManager(Guid? Id=null, string? FirstName=null, string? LastName=null)
        {
            return Employee.Create(
                Id ?? Guid.NewGuid(),
                FirstName ?? "Labor #1 FirstName",
                FirstName ?? "Labor #1 LastName",
                Role.Manager
            );
        }
    }
}