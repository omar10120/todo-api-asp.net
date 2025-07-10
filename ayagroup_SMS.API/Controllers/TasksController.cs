using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.Entities;

using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ayagroup_SMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TasksController(
            ITasksService tasksService,
            IHttpContextAccessor httpContextAccessor)
        {
            _tasksService = tasksService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks(
            [FromQuery] bool? completed,
            [FromQuery] Priority? priority,
            [FromQuery] Guid? categoryId,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!ModelState.IsValid)
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.GetAllAsync(
                    userId, completed, priority, categoryId, search, page, pageSize);

                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            if (!ModelState.IsValid)
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.GetByIdAsync(id, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid)
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();

            try
            {
                
                var result = await _tasksService.CreateAsync(dto, dto.userid);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.UpdateAsync(id, dto, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            if (!ModelState.IsValid)
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.DeleteAsync(id, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ToggleTaskCompletion(Guid id)
        {
            if (!ModelState.IsValid)
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.ToggleCompleteAsync(id, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }
    }
}