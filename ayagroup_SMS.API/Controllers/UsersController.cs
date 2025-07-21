using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.DTOs.Requestes;
using Microsoft.Extensions.Logging;

namespace ayagroup_SMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _usersService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userservice,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UsersController> logger)
        {
            _usersService = userservice;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("GetProfile request started");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {ModelState}", ModelState.ToString());
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                _logger.LogDebug("Fetching profile for user ID: {UserId}", userId);

                var Result = await _usersService.ProfileAsync(userId);
                _logger.LogInformation("Profile retrieved successfully for user ID: {UserId}", userId);

                return Result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile");
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("GetUserById request for ID: {UserId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {ModelState}", ModelState.ToString());
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogDebug("Fetching user with ID: {UserId}", id);
                var Result = await _usersService.GetByIdAsync(id);
                _logger.LogInformation("User retrieved successfully. ID: {UserId}", id);

                return Result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user ID: {UserId}", id);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("GetAllUsers request started");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {ModelState}", ModelState.ToString());
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogDebug("Fetching all users");
                var Result = await _usersService.GetAllUsersAsync();
                
                _logger.LogInformation("Retrieved users");

                return Result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Add([FromBody] RegisterRequestDto register)
        {
            _logger.LogInformation("Register request for username: {Username}", register.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {ModelState}", ModelState.ToString());
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogDebug("Registering new user: {Username}", register.Username);
                var Result = await _usersService.RegisterAsync(register);

                if (Result.Success)
                    _logger.LogInformation("User registered successfully: {Username}", register.Username);
                else
                    _logger.LogWarning("Registration failed: {Message}", Result.Message);

                return Result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Username}", register.Username);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto login)
        {
            _logger.LogInformation("Login request for username: {Username}", login.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {ModelState}", ModelState.ToString());
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogDebug("Attempting login for: {Username}", login.Username);
                var Result = await _usersService.LoginAsync(login);

                if (Result.Success)
                    _logger.LogInformation("Login successful for: {Username}", login.Username);
                else
                    _logger.LogWarning("Login failed for: {Username} - {Message}", login.Username, Result.Message);

                return Result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for: {Username}", login.Username);
                return BadRequest(ex.Message);
            }
        }
    }
}