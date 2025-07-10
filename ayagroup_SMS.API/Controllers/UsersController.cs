using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.DTOs.Requestes;
namespace ayagroup_SMS.API.Controllers

{
    [ApiController]
    [Route("api/[controller]")]




    public class UsersController : ControllerBase
    {

        private readonly IUserService _usersService;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public UsersController(IUserService userservice, IHttpContextAccessor httpContextAccessor)
        {
            _usersService = userservice;
            _httpContextAccessor = httpContextAccessor;
        }




        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> Get()
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                
                var userId = _httpContextAccessor.GetUserId();
                var Result = await _usersService.ProfileAsync(userId);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Owner")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await _usersService.GetByIdAsync(id);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Roles = "Owner")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await _usersService.GetAllUsersAsync();
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("Register")]
        public async Task<IActionResult> Add([FromBody] RegisterRequestDto register)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await _usersService.RegisterAsync(register);
                var Response = Result.ToActionResult();
                return Response;
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> login([FromBody] LoginRequestDto login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await _usersService.LoginAsync(login);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
   
    }

}
