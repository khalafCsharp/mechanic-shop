using Domain.Common.Results;
using Domain.Features.Identity;

namespace MechanicShop.Test.Common.Identity
{
    public static class RefreshTokenFactory
    {

        public static Result<RefreshToken> CreateRefreshToken(Guid? Id = null, string? Token = null, string? UserId = null, DateTimeOffset? ExpiresOnUtc = null)
        {
            return RefreshToken.Create(
                Id ?? Guid.NewGuid(),
                Token ?? "textusedastoken",
                UserId ?? Guid.NewGuid().ToString(),
                ExpiresOnUtc ?? DateTimeOffset.UtcNow.AddDays(2)
            );
        }
    }
}