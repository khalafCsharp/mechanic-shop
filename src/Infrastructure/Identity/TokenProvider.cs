using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Application.Features.Identity.Dtos;
using Azure;
using Domain.Common.Results;
using Domain.Features.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IAppDbContext context;
        private readonly IConfiguration configuration;

        public TokenProvider(IAppDbContext context, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.context = context;
        }
        public async Task<Result<ResponseToken>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default)
        {
            var createTokenResult = await CreateToken(user);

            if (createTokenResult.IsError)
            {
                return createTokenResult.Errors;
            }
            return createTokenResult.Value;
        }

        private async Task<Result<ResponseToken>> CreateToken(AppUserDto user, CancellationToken cancellationToken = default)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            var claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.Sub,user.UserId),
                new (JwtRegisteredClaimNames.Email,user.Email),

            };

            foreach (string role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)), SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(descriptor);

            var oldRefreshTokens = await context.RefreshTokens
              .Where(rt => rt.UserId == user.UserId)
              .ExecuteDeleteAsync();

            var refreshTokenResult = RefreshToken.Create(
                       Guid.NewGuid(),
                       GenerateRefreshToken(),
                       user.UserId,
                       DateTime.UtcNow.AddDays(7));

            if (refreshTokenResult.IsError)
            {
                return refreshTokenResult.Errors;
            }

            context.RefreshTokens.Add(refreshTokenResult.Value);

            await context.SaveChangesAsync(cancellationToken);
            return new ResponseToken
            {
                AccessToken = handler.WriteToken(securityToken),
                RefreshToken = refreshTokenResult.Value.Token,
                ExpirationDate = expires
            };

        }


        private string? GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),
                ValidateIssuer = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JwtSettings:Audience"],
                ValidateLifetime = false, //we know the token already expired
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid expired token.");
            }
            return principal;
        }
    }
}