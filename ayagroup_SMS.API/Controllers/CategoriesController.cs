using ayagroup_SMS.Application.UtilityServices;
using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ayagroup_SMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GetAllCategories request. Search: {Search}, Page: {Page}, PageSize: {PageSize}",
                search, page, pageSize);

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                _logger.LogDebug("User ID: {UserId}", userId);

                var result = await _categoryService.GetAllAsync(search, page, pageSize);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            _logger.LogInformation("GetCategoryById request. ID: {CategoryId}", id);

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                _logger.LogDebug("User ID: {UserId}", userId);

                var result = await _categoryService.GetByIdAsync(id);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category ID: {CategoryId}", id);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            _logger.LogInformation("CreateCategory request. Name: {CategoryName}", dto.Name);

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                _logger.LogDebug("User ID: {UserId}", userId);

                var result = await _categoryService.CreateAsync(dto);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {CategoryName}", dto.Name);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryCreateDto dto)
        {
            _logger.LogInformation("UpdateCategory request. ID: {CategoryId}, New name: {CategoryName}",
                id, dto.Name);

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                _logger.LogDebug("User ID: {UserId}", userId);

                var result = await _categoryService.UpdateAsync(id, dto);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category ID: {CategoryId}", id);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            _logger.LogInformation("DeleteCategory request. ID: {CategoryId}", id);

            try
            {
                var userId = _httpContextAccessor.GetUserId();
                _logger.LogDebug("User ID: {UserId}", userId);

                var result = await _categoryService.DeleteAsync(id);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category ID: {CategoryId}", id);
                return GeneralResponse.InternalError(ex.Message).ToActionResult();
            }
        }
    }
}