using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Features.RepairTasks.Dtos;
using Application.Features.RepairTasks.Mappers;
using Domain.Common.Results;
using Domain.Features.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.RepairTasks.Queries.GetRepairTaskById
{
    public class GetRepairTaskByIdQueryHandler : IRequestHandler<GetRepairTaskByIdQuery, Result<RepairTaskDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<GetRepairTaskByIdQueryHandler> logger;

        public GetRepairTaskByIdQueryHandler(IAppDbContext context, ILogger<GetRepairTaskByIdQueryHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public async Task<Result<RepairTaskDto>> Handle(GetRepairTaskByIdQuery request, CancellationToken cancellationToken)
        {
            RepairTask? repairTask = await context.RepairTasks.AsNoTracking().Include(rt => rt.Parts).FirstOrDefaultAsync(cancellationToken);

            if (repairTask is null)
            {
                logger.LogWarning("repair task with id: {taskId} not found", request.RepairTaskId);
                return ApplicationErrors.RepairTaskNotFound;
            }
            return repairTask.ToDto();
        }
    }
}