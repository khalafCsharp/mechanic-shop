using Api.Controllers;
using Application.Features.Dashboard.Dtos;
using Application.Features.Dashboard.Queries.GetWorkOrderStats;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/dashboard")]
public sealed class DashboardController(ISender sender) : ApiController
{
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DailyWorkOrderStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTodayStats([FromQuery] DateOnly? date, CancellationToken ct)
    {
        var statsDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await sender.Send(new GetWorkOrderStatsQuery(statsDate), ct);

        return result.Match(
            response => Ok(response),
            Problem);
    }
}