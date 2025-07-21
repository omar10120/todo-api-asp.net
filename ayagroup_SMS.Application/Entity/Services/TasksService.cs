
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.Entities;

using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using ayagroup_SMS.Core.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Microsoft.Extensions.Logging;
namespace ayagroup_SMS.Application.EntityServices
{
    public class TasksService : ITasksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TasksService> _logger;
        public TasksService(
            IUnitOfWork unitOfWork,
           ILogger<TasksService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<GeneralResponse> CreateAsync(TaskCreateDto dto, Guid userId)
        {
            _logger.LogInformation("Creating task for user {UserId}. Title: {Title}", userId, dto.Title);

            try
            {
                var task = new Tasks
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    CategoryId = dto.CategoryId,
                    Priority = dto.Priority,
                    UserId = userId
                };

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task created successfully. Task ID: {TaskId}", task.Id);
                return GeneralResponse.Ok("Task created successfully", MapToDto(task));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task for user {UserId}", userId);
                return GeneralResponse.InternalError("Failed to create task");
            }
        }

        public async Task<GeneralResponse> GetByIdAsync(Guid taskId, Guid userId)
        {
            _logger.LogDebug("Getting task by ID: {TaskId} for user {UserId}", taskId, userId);

            try
            {
                var task = await _unitOfWork.Tasks.GetAllAsQueryable()
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

                if (task == null)
                {
                    _logger.LogWarning("Task not found. ID: {TaskId}, User: {UserId}", taskId, userId);
                    return GeneralResponse.NotFound("Task not found");
                }

                _logger.LogInformation("Task retrieved successfully. ID: {TaskId}", taskId);
                return GeneralResponse.Ok("Task received successfully", MapToDto(task));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task by ID: {TaskId}", taskId);
                return GeneralResponse.InternalError("Failed to get task");
            }
        }

        public async Task<GeneralResponse> GetAllAsync(
            Guid userId,
            bool? completed,
            Priority? priority,
            Guid? categoryId,
            string? search,
            int page = 1,
            int pageSize = 10)
        {
            _logger.LogInformation(
                "Getting tasks for user {UserId}. Filters: completed={Completed}, priority={Priority}, " +
                "categoryId={CategoryId}, search={Search}, page={Page}, pageSize={PageSize}",
                userId, completed, priority, categoryId, search, page, pageSize);

            try
            {
                Expression<Func<Tasks, bool>> baseFilter = t => t.UserId == userId;

                if (completed.HasValue)
                    baseFilter = CombineFilters(baseFilter, t => t.IsCompleted == completed.Value);

                if (priority.HasValue)
                    baseFilter = CombineFilters(baseFilter, t => t.Priority == priority.Value);

                if (categoryId.HasValue)
                    baseFilter = CombineFilters(baseFilter, t => t.CategoryId == categoryId.Value);

                if (!string.IsNullOrWhiteSpace(search))
                    baseFilter = CombineFilters(baseFilter,
                        t => t.Title.Contains(search) || t.Description.Contains(search));

                var query = _unitOfWork.Tasks.GetAllAsQueryable()
                    .Where(baseFilter)
                    .Include(t => t.Category)
                    .OrderByDescending(t => t.CreatedAt);

                var totalCount = await query.CountAsync();
                var tasks = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    Tasks = tasks.Select(MapToDto)
                };

                _logger.LogInformation("Returned {TaskCount} tasks for user {UserId}", tasks.Count, userId);
                return GeneralResponse.Ok("Tasks received successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for user {UserId}", userId);
                return GeneralResponse.InternalError("Failed to get tasks");
            }
        }

        public async Task<GeneralResponse> UpdateAsync(Guid taskId, TaskUpdateDto dto, Guid userId)
        {
            _logger.LogInformation("Updating task {TaskId} for user {UserId}", taskId, userId);

            try
            {
                var task = await _unitOfWork.Tasks.GetAllAsQueryable()
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

                if (task == null)
                {
                    _logger.LogWarning("Task not found during update. ID: {TaskId}, User: {UserId}", taskId, userId);
                    return GeneralResponse.NotFound("Task not found");
                }

                // Log changes
                LogChanges(task, dto);

                if (!string.IsNullOrWhiteSpace(dto.Title))
                    task.Title = dto.Title;

                if (dto.Description != null)
                    task.Description = dto.Description;

                if (dto.DueDate.HasValue)
                    task.DueDate = dto.DueDate.Value;

                if (dto.IsCompleted.HasValue)
                    task.IsCompleted = dto.IsCompleted.Value;

                if (dto.CategoryId.HasValue)
                    task.CategoryId = dto.CategoryId.Value;

                if (dto.Priority.HasValue)
                    task.Priority = dto.Priority.Value;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task updated successfully. ID: {TaskId}", taskId);
                return GeneralResponse.Ok("Task updated successfully", MapToDto(task));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", taskId);
                return GeneralResponse.InternalError("Failed to update task");
            }
        }

        public async Task<GeneralResponse> DeleteAsync(Guid taskId, Guid userId)
        {
            _logger.LogInformation("Deleting task {TaskId} for user {UserId}", taskId, userId);

            try
            {
                var task = await _unitOfWork.Tasks.GetAllAsQueryable()
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

                if (task == null)
                {
                    _logger.LogWarning("Task not found during delete. ID: {TaskId}, User: {UserId}", taskId, userId);
                    return GeneralResponse.NotFound("Task not found");
                }

                _unitOfWork.Tasks.Remove(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task deleted successfully. ID: {TaskId}", taskId);
                return GeneralResponse.Ok("Task deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", taskId);
                return GeneralResponse.InternalError("Failed to delete task");
            }
        }

        public async Task<GeneralResponse> ToggleCompleteAsync(Guid taskId, Guid userId)
        {
            _logger.LogInformation("Toggling completion for task {TaskId} for user {UserId}", taskId, userId);

            try
            {
                var task = await _unitOfWork.Tasks.GetAllAsQueryable()
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

                if (task == null)
                {
                    _logger.LogWarning("Task not found during toggle. ID: {TaskId}, User: {UserId}", taskId, userId);
                    return GeneralResponse.NotFound("Task not found");
                }

                var oldStatus = task.IsCompleted;
                task.IsCompleted = !task.IsCompleted;
                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task status toggled. ID: {TaskId}, From: {OldStatus} to {NewStatus}",
                    taskId, oldStatus, task.IsCompleted);

                return GeneralResponse.Ok("Task status updated", new
                {
                    Id = task.Id,
                    IsCompleted = task.IsCompleted
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling task status {TaskId}", taskId);
                return GeneralResponse.InternalError("Failed to toggle task status");
            }
        }

        private void LogChanges(Tasks task, TaskUpdateDto dto)
        {
            var changes = new List<string>();

            if (!string.IsNullOrWhiteSpace(dto.Title) && dto.Title != task.Title)
                changes.Add($"Title: '{task.Title}' → '{dto.Title}'");

            if (dto.Description != null && dto.Description != task.Description)
                changes.Add($"Description updated");

            if (dto.DueDate.HasValue && dto.DueDate.Value != task.DueDate)
                changes.Add($"DueDate: '{task.DueDate}' → '{dto.DueDate}'");

            if (dto.IsCompleted.HasValue && dto.IsCompleted.Value != task.IsCompleted)
                changes.Add($"Status: {task.IsCompleted} → {dto.IsCompleted}");

            if (dto.CategoryId.HasValue && dto.CategoryId.Value != task.CategoryId)
                changes.Add($"Category: {task.CategoryId} → {dto.CategoryId}");

            if (dto.Priority.HasValue && dto.Priority.Value != task.Priority)
                changes.Add($"Priority: {task.Priority} → {dto.Priority}");

            if (changes.Any())
            {
                _logger.LogInformation("Updating task {TaskId} with changes: {Changes}",
                    task.Id, string.Join(", ", changes));
            }
        }

      
      
    
        private TaskResponseDto MapToDto(Tasks task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                Category = task.Category?.Name
            };
        }

        private Expression<Func<T, bool>> CombineFilters<T>(
            Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter));

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }
}