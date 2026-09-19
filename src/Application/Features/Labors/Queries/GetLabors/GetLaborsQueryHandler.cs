using Application.Common.Interfaces;
using Application.Features.Labors.Dtos;
using Application.Features.Labors.Mappers;
using Domain.Common.Results;
using Domain.Features.Employees;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Labors.Queries.GetLabors
{
    public class GetLaborsQueryHandler : IRequestHandler<GetLaborsQuery, Result<List<LaborDto>>>
    {
        private readonly IAppDbContext context;

        public GetLaborsQueryHandler(IAppDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<List<LaborDto>>> Handle(GetLaborsQuery request, CancellationToken cancellationToken)
        {
            List<Employee> labors = await context.Employees.AsNoTracking().Where(e => e.Role == Role.Labor).ToListAsync(cancellationToken);

            return labors.ToDtos();
        }
    }
}