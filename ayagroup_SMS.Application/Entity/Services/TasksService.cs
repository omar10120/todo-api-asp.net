
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.Entities;

using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using ayagroup_SMS.Core.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ayagroup_SMS.Core.conts;
namespace ayagroup_SMS.Application.EntityServices
{
    public class TasksService : ITasksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TasksService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GeneralResponse> CreateAsync(TaskCreateDto dto, Guid userId)
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

            return GeneralResponse.Ok("Task created successfully", MapToDto(task));
        }

        public async Task<GeneralResponse> GetByIdAsync(Guid taskId, Guid userId)
        {
            var task =  await _unitOfWork.Tasks.GetAllAsQueryable()
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return GeneralResponse.NotFound("Task not found");

            return GeneralResponse.Ok("Task received  successfully ", MapToDto(task));
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

            return GeneralResponse.Ok("Tasks received  successfully", result);
        }

        public async Task<GeneralResponse> UpdateAsync(Guid taskId, TaskUpdateDto dto, Guid userId)
        {
            var task = await _unitOfWork.Tasks.GetAllAsQueryable()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return GeneralResponse.NotFound("Task not found");

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

            return GeneralResponse.Ok("Task updated successfully", MapToDto(task));
        }

        public async Task<GeneralResponse> DeleteAsync(Guid taskId, Guid userId)
        {
            var task = await _unitOfWork.Tasks.GetAllAsQueryable()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return GeneralResponse.NotFound("Task not found");

            _unitOfWork.Tasks.Remove(task);
            await _unitOfWork.SaveChangesAsync();

            return GeneralResponse.Ok("Task deleted successfully");
        }

        public async Task<GeneralResponse> ToggleCompleteAsync(Guid taskId, Guid userId)
        {
            var task = await _unitOfWork.Tasks.GetAllAsQueryable()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return GeneralResponse.NotFound("Task not found");

            task.IsCompleted = !task.IsCompleted;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();

            return GeneralResponse.Ok("Task status updated", new
            {
                Id = task.Id,
                IsCompleted = task.IsCompleted
            });
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