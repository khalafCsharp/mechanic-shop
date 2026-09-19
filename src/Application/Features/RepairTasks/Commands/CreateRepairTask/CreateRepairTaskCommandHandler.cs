using Application.Common.Interfaces;
using Application.Features.RepairTasks.Commands.CreateRepairTask;
using Application.Features.RepairTasks.Dtos;
using Application.Features.RepairTasks.Mappers;
using Domain.Common.Results;
using Domain.Features.RepairTasks;
using Domain.Features.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.RepairTasks.Requests.CreateRepairTask
{
    public class CreateRepairTaskCommandHandler : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
    {
        private readonly IAppDbContext context;
        private readonly ILogger<CreateRepairTaskCommandHandler> logger;
        private readonly HybridCache cache;

        public CreateRepairTaskCommandHandler(IAppDbContext context, ILogger<CreateRepairTaskCommandHandler> logger, HybridCache cache)
        {
            this.cache = cache;
            this.context = context;
            this.logger = logger;
        }
        public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand request, CancellationToken cancellationToken)
        {
            bool isNewRepairTaskNameAlreadyExists = await context.RepairTasks.AnyAsync(rt=>EF.Functions.Like(rt.Name,request.Name));

            if(isNewRepairTaskNameAlreadyExists)
            {
                logger.LogWarning("a repair task with almost the same Name already exists, Name: {name}", request.Name);

                return RepairTaskErrors.DuplicateName;
            }
            List<Part> parts = [];

            foreach (CreatePartCommand cpc in request.CreatePartCommands)
            {
                Result<Part> createPartResult = Part.Create(Guid.NewGuid(), cpc.Name, cpc.Cost, cpc.Quantity);

                if (createPartResult.IsError)
                {
                    return createPartResult.Errors;
                }

                parts.Add(createPartResult.Value);
            }

            var createRepairTaskResult = RepairTask.Create(
                        id: Guid.NewGuid(),
                        name: request.Name!,
                        laborCost: request.LaborCost,
                        estimatedDurationInMins: request.TaskDuration,
                        parts: parts);

            if (createRepairTaskResult.IsError)
            {
                return createRepairTaskResult.Errors;
            }

            var repairTask = createRepairTaskResult.Value;

            context.RepairTasks.Add(repairTask);

            await context.SaveChangesAsync(cancellationToken);

            await cache.RemoveByTagAsync("repair-tasks", cancellationToken);

            return repairTask.ToDto();
        }
    }
}