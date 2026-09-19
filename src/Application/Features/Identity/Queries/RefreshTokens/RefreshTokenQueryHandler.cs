using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Interfaces;
using Application.Features.Identity.Dtos;
using Domain.Common.Results;
using Domain.Features.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.Identity.Queries.RefreshTokens
{
    public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, Result<ResponseToken>>
    {
        private readonly ILogger<RefreshTokenQueryHandler> logger;
        private readonly IIdentityService identityService;
        private readonly ITokenProvider tokenProvider;
        private readonly IAppDbContext context;

        public RefreshTokenQueryHandler(ILogger<RefreshTokenQueryHandler> logger, IIdentityService identityService, ITokenProvider tokenProvider, IAppDbContext context)
        {
            this.identityService = identityService;
            this.tokenProvider = tokenProvider;
            this.context = context;
            this.logger = logger;

        }
        public async Task<Result<ResponseToken>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal? principal = tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);

            if (principal is null)
            {
                logger.LogError("expired access token is not valid");
                return ApplicationErrors.ExpiredAccessTokenInvalid;
            }
            string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                logger.LogError("invalid user id");
                return ApplicationErrors.UserIdClaimInvalid;
            }
            Result<AppUserDto> getUserByItsIdResult = await identityService.GetUserByIdAsync(userId);

            if (getUserByItsIdResult.IsError)
            {
                logger.LogError("error occured when getting user: {getUserResultTopErrorDescription}", getUserByItsIdResult.TopError.Description);
                return getUserByItsIdResult.Errors;
            }

            RefreshToken? refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken && r.UserId == userId, cancellationToken);

            if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            {
                logger.LogError("refresh token has expired");
                return ApplicationErrors.RefreshTokenExpired;
            }
            Result<ResponseToken> generateJwtTokenResult = await tokenProvider.GenerateJwtTokenAsync(getUserByItsIdResult.Value, cancellationToken);

            if (generateJwtTokenResult.IsError)
            {
                logger.LogError("error occurred while generating jwt token, description: {topErrorDescription}", generateJwtTokenResult.TopError.Description);

                return generateJwtTokenResult.Errors;
            }
            return generateJwtTokenResult.Value;
        }
    }
}