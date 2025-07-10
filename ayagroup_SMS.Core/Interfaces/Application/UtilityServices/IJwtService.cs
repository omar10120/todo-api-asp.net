
using ayagroup_SMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
namespace ayagroup_SMS.Core.Interfaces.Application.UtilityServices
{
    public interface IJwtService
    {


        Task<string> GenerateToken(User user, UserManager<User> userManager);
    }
}
