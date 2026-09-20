using Application.Features.WorkOrders.Dtos;
using Domain.Common.Results;
using Domain.Features.WorkOrders.Enums;
using MediatR;

namespace Application.Features.WorkOrders.Commands.CreateWorkOrder
{
    public record CreateWorkOrderCommand(Spot Spot, Guid VehicleId, DateTimeOffset StartAtUtc, List<Guid> RepairTaskIds, Guid? LaborId) : IRequest<Result<WorkOrderDto>>;

}