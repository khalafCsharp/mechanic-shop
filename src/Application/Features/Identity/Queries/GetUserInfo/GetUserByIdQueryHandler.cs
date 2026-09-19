using Application.Common.Interfaces;
using Application.Features.Identity.Dtos;
using Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Identity.Queries.GetUserInfo
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<AppUserDto>>
    {
        private readonly ILogger<GetUserByIdQueryHandler> logger;
        private readonly IIdentityService identityService;
        public GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger, IIdentityService identityService)
        {
            this.identityService = identityService;
            this.logger = logger;
        }
        public async Task<Result<AppUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            Result<AppUserDto> getUserByIdResult = await identityService.GetUserByIdAsync(request.UserId);

            if (getUserByIdResult.IsError)
            {
                logger.LogError("error while getting user with id: {userId}, error description: {topErrorDescription}", request.UserId, getUserByIdResult.TopError.Description);

                return getUserByIdResult.Errors;
            }
            return getUserByIdResult.Value;
        }
    }
}