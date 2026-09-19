using Application.Features.Identity.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.Identity.Queries.RefreshTokens
{
    public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<ResponseToken>>;
}