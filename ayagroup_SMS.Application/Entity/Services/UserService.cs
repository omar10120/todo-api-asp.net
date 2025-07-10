using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using Microsoft.AspNetCore.Identity;
using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Application.UtilityServices;
using ayagroup_SMS.Core.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Http;
using ayagroup_SMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using System.Data;
using ayagroup_SMS.Core.conts;
using Azure.Core;
namespace ayagroup_SMS.Application.Entity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        private readonly IJwtService _jwtService;
        
        private readonly IUnitOfWork db;

        public UserService(UserManager<User> userManager,
         IUnitOfWork db,
         IJwtService jwtService,


         IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _jwtService = jwtService;
         

            this.db = db;
        }

        public async Task<GeneralResponse> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
                return GeneralResponse.BadRequest("Email is already in use.");

            if (await _userManager.Users.AnyAsync(s => s.UserName == registerRequest.Username))
                return GeneralResponse.BadRequest("Name is already taken.");

            var user = new User
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                EmailConfirmed = true,
                NormalizedUserName = registerRequest.Username.ToUpper(),
                

            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
                return GeneralResponse.BadRequest($"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");


            if (!Enum.IsDefined(typeof(Roles), registerRequest.Roles))
                return GeneralResponse.BadRequest("Invalid role specified");

            var roleName = registerRequest.Roles.GetDescription(); 

            await _userManager.AddToRoleAsync(user, roleName);
            var token = await _jwtService.GenerateToken(user, _userManager);
            var data = new LoginResponseDto() { Token = token };
            
            return GeneralResponse.Ok("User created successfully", data);
        }

        public async Task<GeneralResponse> LoginAsync(LoginRequestDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                    return GeneralResponse.BadRequest("Username and password are required.");

             
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u =>
                        u.UserName.ToLower() == loginDto.Username.ToLower() ||
                        u.Email.ToLower() == loginDto.Username.ToLower());

                if (user == null)
                    return GeneralResponse.BadRequest("Invalid username or email.");

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isPasswordValid)
                    return GeneralResponse.BadRequest("Invalid password.");

                var token = _jwtService.GenerateToken(user, _userManager);
            

                    return GeneralResponse.Ok("Login successful", new
                    {
                        Token = token
                    });
              
            }
            catch (Exception ex)
            {
                return GeneralResponse.BadRequest($"Unexpected error: {ex.Message}");
            }
        }
        public async Task<GeneralResponse> ProfileAsync(Guid userid)
        {
            var ErrorMsg = "";
            try
            {

                var user = await _userManager.Users.Where(x=>x.Id == userid).FirstOrDefaultAsync();
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();
                    var userdto = new UserDto
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt,
                        Role = role!,

                    };

                    return GeneralResponse.Ok("User received successfully" + ErrorMsg, userdto);
                }
                return GeneralResponse.NotFound("User not found");



            }
            catch (DbUpdateException dbEx)
            {
                return GeneralResponse.BadRequest($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return GeneralResponse.BadRequest($"Unexpected error: {ex.Message}");
            }
        }
        public async Task<GeneralResponse> GetByIdAsync(Guid id)
        {
            try
            {
                var ErrorMsg = "";
                if (string.IsNullOrEmpty(id.ToString()))
                {
                    return GeneralResponse.BadRequest("id is required");
                }
                
                var user = await _userManager.Users.Where(x => x.Id == id).FirstOrDefaultAsync();


                return GeneralResponse.Ok("user received successfully" + ErrorMsg, user);
            }

            catch (DbUpdateException dbEx)
            {
                return GeneralResponse.BadRequest($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return GeneralResponse.BadRequest($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (users == null || !users.Any())
                    return GeneralResponse.NotFound("No users found");

                return GeneralResponse.Ok("Users retrieved successfully", users);
            }
            catch (DbUpdateException dbEx)
            {
                return GeneralResponse.InternalError($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Unexpected error: {ex.Message}");
            }
        }
    }
}
