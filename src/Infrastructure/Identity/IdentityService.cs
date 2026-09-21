using Application.Common.Interfaces;
using Application.Features.Identity.Dtos;
using Domain.Common.Results;
using Domain.Common.Results.Errors;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> userManager;
        public IdentityService(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;

        }
        public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
        {
            AppUser? user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Error.NotFound("Email_Not_Found", $"user with email {UtilityService.MaskEmail(email)} not found");
            }
            if (!user.EmailConfirmed)
            {
                return Error.Conflict("Email_Not_Confirmed", $"email '{UtilityService.MaskEmail(email)}' not confirmed");
            }
            if (!await userManager.CheckPasswordAsync(user, password))
            {
                return Error.Conflict("Invalid_Login_Try", $"wrong login credentials");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            return new AppUserDto(user.Id, user.Email!, userRoles, userClaims);
        }

        public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException(nameof(userId));

            var userRoles = await userManager.GetRolesAsync(user);

            var userClaims = await userManager.GetClaimsAsync(user);

            return new AppUserDto(user.Id, user.Email!, userRoles, userClaims);
        }

        public async Task<string?> GetUserNameAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            return user?.UserName;
        }
    }
}