

using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;

namespace ayagroup_SMS.Core.Interfaces.Application.EntityServices
{
    public interface ICategoryService
    {
        Task<GeneralResponse> CreateAsync(CategoryCreateDto dto);
        Task<GeneralResponse> GetByIdAsync(Guid categoryId);
        Task<GeneralResponse> GetAllAsync( string? search = null, int page = 1, int pageSize = 10);
        Task<GeneralResponse> UpdateAsync(Guid categoryId, CategoryCreateDto dto);
        Task<GeneralResponse> DeleteAsync(Guid categoryId);
    }
}
