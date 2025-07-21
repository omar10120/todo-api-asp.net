using ayagroup_SMS.Application.EntityServices;
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using ayagroup_SMS.Core.Interfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ayagroup_SMS.Application.Entity.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            IUnitOfWork unitOfWork,
            ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<GeneralResponse> CreateAsync(CategoryCreateDto dto)
        {
            _logger.LogInformation("Creating category: {CategoryName}", dto.Name);

            try
            {
                var category = new Category
                {
                    Name = dto.Name
                };

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category created successfully. ID: {CategoryId}, Name: {CategoryName}",
                    category.Id, category.Name);

                return GeneralResponse.Ok("Category created successfully", MapToDto(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {CategoryName}", dto.Name);
                return GeneralResponse.InternalError("Failed to create category");
            }
        }

        public async Task<GeneralResponse> GetByIdAsync(Guid categoryId)
        {
            _logger.LogInformation("Getting category by ID: {CategoryId}", categoryId);

            try
            {
                var category = await _unitOfWork.Categories.GetAllAsQueryable()
                    .Where(x => x.Id == categoryId)
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    _logger.LogWarning("Category not found. ID: {CategoryId}", categoryId);
                    return GeneralResponse.NotFound("Category not found");
                }

                _logger.LogInformation("Category retrieved successfully. ID: {CategoryId}", categoryId);
                return GeneralResponse.Ok("Category retrieved successfully", MapToDto(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category ID: {CategoryId}", categoryId);
                return GeneralResponse.InternalError("Failed to get category");
            }
        }

        public async Task<GeneralResponse> GetAllAsync(
            string? search = null,
            int page = 1,
            int pageSize = 10)
        {
            _logger.LogInformation("Getting categories. Search: {Search}, Page: {Page}, PageSize: {PageSize}",
                search, page, pageSize);

            try
            {
                var query = _unitOfWork.Categories.GetAllAsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    _logger.LogDebug("Applying search filter: {Search}", search);
                    query = query.Where(c => c.Name.Contains(search));
                }

                var totalCount = await query.CountAsync();
                var categories = await query
                    .OrderBy(c => c.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new
                {
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    Categories = categories.Select(MapToDto)
                };

                _logger.LogInformation("Retrieved {Count} categories", categories.Count);
                return GeneralResponse.Ok("Categories retrieved successfully", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return GeneralResponse.InternalError("Failed to get categories");
            }
        }

        public async Task<GeneralResponse> UpdateAsync(Guid categoryId, CategoryCreateDto dto)
        {
            _logger.LogInformation("Updating category ID: {CategoryId}", categoryId);

            try
            {
                var category = await _unitOfWork.Categories.GetAllAsQueryable()
                    .FirstOrDefaultAsync(c => c.Id == categoryId);

                if (category == null)
                {
                    _logger.LogWarning("Category not found during update. ID: {CategoryId}", categoryId);
                    return GeneralResponse.NotFound("Category not found");
                }

                var oldName = category.Name;
                category.Name = dto.Name;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category updated. ID: {CategoryId}, Name: '{OldName}' → '{NewName}'",
                    categoryId, oldName, dto.Name);

                return GeneralResponse.Ok("Category updated successfully", MapToDto(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category ID: {CategoryId}", categoryId);
                return GeneralResponse.InternalError("Failed to update category");
            }
        }

        public async Task<GeneralResponse> DeleteAsync(Guid categoryId)
        {
            _logger.LogInformation("Deleting category ID: {CategoryId}", categoryId);

            try
            {
                var category = await _unitOfWork.Categories.GetAllAsQueryable()
                    .Include(c => c.Tasks)
                    .FirstOrDefaultAsync(c => c.Id == categoryId);

                if (category == null)
                {
                    _logger.LogWarning("Category not found during delete. ID: {CategoryId}", categoryId);
                    return GeneralResponse.NotFound("Category not found");
                }

                var taskCount = category.Tasks.Count;
                foreach (var task in category.Tasks)
                {
                    task.CategoryId = null;
                }

                _unitOfWork.Categories.Remove(category);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category deleted. ID: {CategoryId}, Name: {CategoryName}, Affected tasks: {TaskCount}",
                    categoryId, category.Name, taskCount);

                return GeneralResponse.Ok("Category deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category ID: {CategoryId}", categoryId);
                return GeneralResponse.InternalError("Failed to delete category");
            }
        }

        private CategoryResponseDto MapToDto(Category category)
        {
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
            };
        }
    }
}