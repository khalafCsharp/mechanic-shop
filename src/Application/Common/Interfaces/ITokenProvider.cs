using System.Security.Claims;
using Application.Features.Identity.Dtos;
using Domain.Common.Results;

namespace Application.Common.Interfaces
{
    public interface ITokenProvider
    {
        Task<Result<ResponseToken>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}