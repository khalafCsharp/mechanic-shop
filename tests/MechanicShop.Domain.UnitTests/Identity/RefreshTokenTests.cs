using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Identity;
using MechanicShop.Test.Common.Identity;

namespace MechanicShop.Domain.UnitTests.Identity
{
    public class RefreshTokenTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Given
            var rf = RefreshTokenFactory.CreateRefreshToken().Value;


            // When
            var result = RefreshTokenFactory.CreateRefreshToken(rf.Id, rf.Token, rf.UserId, rf.ExpiresOnUtc);
            // Then

            Assert.True(result.IsSuccess);
            Assert.True(result.Value.ExpiresOnUtc > DateTimeOffset.UtcNow);

            Assert.Equal(rf.Token, result.Value.Token);
            Assert.Equal(rf.ExpiresOnUtc, result.Value.ExpiresOnUtc);
            Assert.Equal(rf.UserId, result.Value.UserId);
        }
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Create_WithInvalidToken_ShouldFail(string invalidToken)
        {
            var result = RefreshTokenFactory.CreateRefreshToken(Token: invalidToken);

            Assert.True(result.IsError);
            Assert.Equal(RefreshTokenErrors.TokenRequired.Code, result.TopError.Code);

        }
        [Fact]
        public void Create_WithEmtyId_ShouldFail()
        {

            // When
            var result = RefreshTokenFactory.CreateRefreshToken(Id: Guid.Empty);
            // Then

            Assert.True(result.IsError);
            Assert.Equal(RefreshTokenErrors.IdRequired.Code, result.TopError.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]//userId in refreshToken is string of guid
        public void Create_WithInvalidUserId_ShouldFail(string? invalidUserId)
        {
            var result = RefreshTokenFactory.CreateRefreshToken(UserId: invalidUserId);

            Assert.True(result.IsError);

            Assert.Equal("RefreshToken_UserId_Required", result.TopError.Code);
        }

        [Fact]
        public void Create_WithExpirationExpired_ShouldFail()
        {
            var result = RefreshTokenFactory.CreateRefreshToken(ExpiresOnUtc: DateTimeOffset.UtcNow.AddHours(-1));

            Assert.True(result.IsError);
            Assert.Equal(RefreshTokenErrors.ExpiryInvalid.Code, result.TopError.Code);
        }
    }
}