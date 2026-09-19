using Application.Common.Interfaces;
using Application.Features.Identity.Dtos;
using Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.Identity.Queries.GenerateTokens
{
    public class GenerateTokenQueryHandler : IRequestHandler<GenerateTokenQuery, Result<ResponseToken>>
    {
        private readonly IIdentityService identityService;
        private readonly ITokenProvider tokenProvider;
        private readonly ILogger<GenerateTokenQueryHandler> logger;

        public GenerateTokenQueryHandler(IIdentityService identityService, ITokenProvider tokenProvider, ILogger<GenerateTokenQueryHandler> logger)
        {
            this.logger = logger;
            this.identityService = identityService;
            this.tokenProvider = tokenProvider;
        }
        public async Task<Result<ResponseToken>> Handle(GenerateTokenQuery request, CancellationToken cancellationToken)
        {
            Result<AppUserDto> authenticateUserResult = await identityService.AuthenticateAsync(request.Email, request.Password);

            if (authenticateUserResult.IsError)
            {
                return authenticateUserResult.Errors;
            }

            Result<ResponseToken> generateJwtTokenResult = await tokenProvider.GenerateJwtTokenAsync(authenticateUserResult.Value, cancellationToken);

            if (generateJwtTokenResult.IsError)
            {
                logger.LogError("an error occured during generating jwt token, description: {topErrorDescription}", generateJwtTokenResult.TopError.Description);

                return generateJwtTokenResult.Errors;
            }


            return generateJwtTokenResult.Value;
        }
    }
}