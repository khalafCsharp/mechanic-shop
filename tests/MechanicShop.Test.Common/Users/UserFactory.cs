using Infrastructure.Identity;

namespace MechanicShop.Test.Common.Users
{
    public static class UserFactory
    {
        public static AppUser CreateUser(Guid? Id=null,string? Email=null,string? UserName=null,bool EmailConfirmid=true)
        {
            return new AppUser
            {
                Id = "14a59129-6c20-417a-839d-11a708d32d96",
                Email = "khalaf@gmail.com",
                UserName = "khalifa",
                EmailConfirmed = true
            };
        }
    }

}