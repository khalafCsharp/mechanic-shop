using Application.Features.Identity.Dtos;
using Domain.Common.Results;
using MediatR;

namespace Application.Features.Identity.Queries.GetUserInfo
{
    public record GetUserByIdQuery(string UserId) : IRequest<Result<AppUserDto>>;
}