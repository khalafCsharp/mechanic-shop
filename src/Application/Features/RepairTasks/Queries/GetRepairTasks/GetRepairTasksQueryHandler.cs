using Application.Common.Interfaces;
using Application.Features.RepairTasks.Dtos;
using Application.Features.RepairTasks.Mappers;
using Domain.Common.Results;
using Domain.Features.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.RepairTasks.Queries.GetRepairTasks
{
    public class GetRepairTasksQueryHandler : IRequestHandler<GetRepairTasksQuery, Result<List<RepairTaskDto>>>
    {
        private readonly IAppDbContext context;
        public GetRepairTasksQueryHandler(IAppDbContext context)
        {
            this.context = context;

        }
        public async Task<Result<List<RepairTaskDto>>> Handle(GetRepairTasksQuery request, CancellationToken cancellationToken)
        {
            List<RepairTask> repairTasks = await context.RepairTasks.AsNoTracking().Include(rt => rt.Parts).ToListAsync(cancellationToken);

            return repairTasks.ToDtos();
        }
    }
}