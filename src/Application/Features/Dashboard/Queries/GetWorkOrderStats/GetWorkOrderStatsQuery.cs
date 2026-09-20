using Application.Features.Dashboard.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.Dashboard.Queries.GetWorkOrderStats
{
    public record GetWorkOrderStatsQuery(DateOnly Date) : IRequest<Result<DailyWorkOrderStatsDto>>;
}