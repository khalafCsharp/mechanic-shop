using Application.Features.Labors.Dtos;
using Domain.Features.Employees;

namespace Application.Features.Labors.Mappers
{
    public static class LaborMapper
    {
        public static LaborDto ToDto(this Employee? employee)
        {
            if (employee is null)
            {
                throw new NullReferenceException("employee is null");

            }
            return new LaborDto(employee.Id, employee.FullName);

        }

        public static List<LaborDto> ToDtos(this IEnumerable<Employee> employees)
        {
            return [.. employees.Select(e => e.ToDto())];
        }
    }
}