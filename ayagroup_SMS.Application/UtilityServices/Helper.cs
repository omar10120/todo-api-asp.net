using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ayagroup_SMS.Application.UtilityServices;
using System.Security.Claims;
using ayagroup_SMS.Core.DTOs.Responses;

namespace ayagroup_SMS.Application.UtilityServices
{
    public static class Helper
    {
        public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            var userIdClaim = user?.FindFirst("Id");
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
            

        }
        public static string GetUserRole(this IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            var userIdClaim = user?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            return userIdClaim != null ? userIdClaim.Value : null!;

        }
        public static IActionResult ToActionResult(this GeneralResponse response)

        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

    }
}
