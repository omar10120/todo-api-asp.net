using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using Microsoft.AspNetCore.Identity;
using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Application.UtilityServices;
using ayagroup_SMS.Core.Interfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.conts;
using Microsoft.Extensions.Logging;

namespace ayagroup_SMS.Application.Entity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<GeneralResponse> RegisterAsync(RegisterRequestDto registerRequest)
        {
            _logger.LogInformation("Registering user: {Email}", registerRequest.Email);

            try
            {
                // Check for existing email
                if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
                {
                    _logger.LogWarning("Registration failed - Email already in use: {Email}", registerRequest.Email);
                    return GeneralResponse.BadRequest("Email is already in use.");
                }

                // Check for existing username
                if (await _userManager.Users.AnyAsync(s => s.UserName == registerRequest.Username))
                {
                    _logger.LogWarning("Registration failed - Username taken: {Username}", registerRequest.Username);
                    return GeneralResponse.BadRequest("Name is already taken.");
                }

                // Create user
                var user = new User
                {
                    UserName = registerRequest.Username,
                    Email = registerRequest.Email,
                    EmailConfirmed = true,
                    NormalizedUserName = registerRequest.Username.ToUpper(),
                };

                var result = await _userManager.CreateAsync(user, registerRequest.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("User creation failed: {Errors}", errors);
                    return GeneralResponse.BadRequest($"Registration failed: {errors}");
                }

                // Validate role
                if (!Enum.IsDefined(typeof(Roles), registerRequest.Roles))
                {
                    _logger.LogWarning("Invalid role specified: {Role}", registerRequest.Roles);
                    return GeneralResponse.BadRequest("Invalid role specified");
                }

                var roleName = registerRequest.Roles.GetDescription();
                await _userManager.AddToRoleAsync(user, roleName);

                var token = await _jwtService.GenerateToken(user, _userManager);

                _logger.LogInformation("User registered successfully. ID: {UserId}, Username: {Username}",
                    user.Id, registerRequest.Username);

                return GeneralResponse.Ok("User created successfully", new LoginResponseDto { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Email}", registerRequest.Email);
                return GeneralResponse.InternalError("Registration failed due to an unexpected error");
            }
        }

        public async Task<GeneralResponse> LoginAsync(LoginRequestDto loginDto)
        {
            _logger.LogInformation("Login attempt for: {Identifier}", loginDto.Username);

            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    _logger.LogWarning("Login failed - Missing credentials");
                    return GeneralResponse.BadRequest("Username and password are required.");
                }

                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u =>
                        u.UserName.ToLower() == loginDto.Username.ToLower() ||
                        u.Email.ToLower() == loginDto.Username.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Login failed - User not found: {Identifier}", loginDto.Username);
                    return GeneralResponse.BadRequest("Invalid username or email.");
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Login failed - Invalid password for: {Identifier}", loginDto.Username);
                    return GeneralResponse.BadRequest("Invalid password.");
                }

                var token = await _jwtService.GenerateToken(user, _userManager);
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("Login successful. User ID: {UserId}, Roles: {Roles}",
                    user.Id, string.Join(", ", roles));

                return GeneralResponse.Ok("Login successful", new
                {
                    Token = token,
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for: {Identifier}", loginDto.Username);
                return GeneralResponse.InternalError("Login failed due to an unexpected error");
            }
        }

        public async Task<GeneralResponse> ProfileAsync(Guid userid)
        {
            _logger.LogInformation("Fetching profile for user ID: {UserId}", userid);

            try
            {
                var user = await _userManager.Users
                    .Where(x => x.Id == userid)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning("Profile not found for user ID: {UserId}", userid);
                    return GeneralResponse.NotFound("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                _logger.LogDebug("Profile retrieved for user ID: {UserId}", userid);

                return GeneralResponse.Ok("User received successfully", new UserDto
                {
                    Username = user.UserName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    Role = role!
                });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error fetching profile for user ID: {UserId}", userid);
                return GeneralResponse.InternalError($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching profile for user ID: {UserId}", userid);
                return GeneralResponse.InternalError($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Get user by ID request: {UserId}", id);

            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid user ID requested");
                    return GeneralResponse.BadRequest("ID is required");
                }

                var user = await _userManager.Users
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", id);
                    return GeneralResponse.NotFound("User not found");
                }

                _logger.LogInformation("User retrieved: {UserId}", id);
                return GeneralResponse.Ok("User received successfully", user);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error fetching user ID: {UserId}", id);
                return GeneralResponse.InternalError($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user ID: {UserId}", id);
                return GeneralResponse.InternalError($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetAllUsersAsync()
        {
            _logger.LogInformation("GetAllUsers request");

            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found in database");
                    return GeneralResponse.NotFound("No users found");
                }

                _logger.LogInformation("Retrieved {Count} users", users.Count);
                return GeneralResponse.Ok("Users retrieved successfully", users);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error fetching all users");
                return GeneralResponse.InternalError($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return GeneralResponse.InternalError($"Unexpected error: {ex.Message}");
            }
        }
    }
}