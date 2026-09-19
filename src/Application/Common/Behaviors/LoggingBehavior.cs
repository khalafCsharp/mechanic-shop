using System.Data.Common;
using Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest>> _logger;
        private readonly IUser _user;
        private readonly IIdentityService _identityService;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest>> logger, IUser user, IIdentityService identityService)
        {
            this._logger = logger;
            this._user = user;
            this._identityService = identityService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;
            string userId = _user.Id ?? string.Empty;
            string? userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.LogInformation("Request: {requestName} {userId} {userName} {@request}", requestName, userId, userName, request);
        }
    }
}