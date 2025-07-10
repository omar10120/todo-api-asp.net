using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Application.UtilityServices;
using ayagroup_SMS.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace ayagroup_SMS.Application.UtilityServices
{
    public class JwtService : IJwtService
    {


        public async Task<string> GenerateToken(User user, UserManager<User> userManager)
        {

            var jwtSettings = ProjectConfig.Instance.JwtSettings; 
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", user.UserName),
                new Claim("Email", user.Email),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                 issuer: jwtSettings.Issuer,
                 audience: jwtSettings.Audience,
                 claims: claims,
                 expires: DateTime.Now.AddDays(jwtSettings.ExpiryMinutes),
                 signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
