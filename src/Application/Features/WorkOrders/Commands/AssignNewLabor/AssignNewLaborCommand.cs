using Domain.Common.Results;
using MediatR;

namespace Application.Features.WorkOrders.Commands.AssignNewLabor
{
    public record AssignNewLaborCommand(Guid WorkOrderId, Guid LaborId) : IRequest<Result<Updated>>;
}