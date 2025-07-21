// Updated TasksController with logging
using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Add this

namespace ayagroup_SMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TasksController> _logger; // Add logger

        public TasksController(
            ITasksService tasksService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<TasksController> logger) // Inject logger
        {
            _tasksService = tasksService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
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
            _logger.LogInformation(
                "GetAllTasks request: completed={Completed}, priority={Priority}, " +
                "categoryId={CategoryId}, search={Search}, page={Page}, pageSize={PageSize}",
                completed, priority, categoryId, search, page, pageSize);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {ModelState}", ModelState.ToString());
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();
            }

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                _logger.LogDebug("User ID: {UserId}", userId);

                var result = await _tasksService.GetAllAsync(
                    userId, completed, priority, categoryId, search, page, pageSize);

                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllTasks");
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            _logger.LogInformation("GetTaskById request: ID={TaskId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task {TaskId}: {ModelState}", id, ModelState.ToString());
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();
            }

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.GetByIdAsync(id, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task {TaskId}", id);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto dto)
        {
            _logger.LogInformation("CreateTask request: Title={Title}", dto.Title);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task creation: {ModelState}", ModelState.ToString());
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();
            }

            try
            {
                var userId = dto.userid;
                if (userId == null || userId == Guid.Empty)
                    userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.CreateAsync(dto, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task"+ "userId:" );
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskUpdateDto dto)
        {
            _logger.LogInformation("UpdateTask request: ID={TaskId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task update {TaskId}: {ModelState}", id, ModelState.ToString());
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();
            }

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.UpdateAsync(id, dto, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            _logger.LogInformation("DeleteTask request: ID={TaskId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task deletion {TaskId}: {ModelState}", id, ModelState.ToString());
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();
            }

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.DeleteAsync(id, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPatch("{id}/complete")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ToggleTaskCompletion(Guid id)
        {
            _logger.LogInformation("ToggleTaskCompletion request: ID={TaskId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task toggle {TaskId}: {ModelState}", id, ModelState.ToString());
                return GeneralResponse.BadRequest(ModelState.ToString()).ToActionResult();
            }

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                var result = await _tasksService.ToggleCompleteAsync(id, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling task status {TaskId}", id);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }
    }
}