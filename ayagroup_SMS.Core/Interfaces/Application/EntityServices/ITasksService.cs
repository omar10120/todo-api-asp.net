

using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
using ayagroup_SMS.Core.Entities;
namespace ayagroup_SMS.Core.Interfaces.Application.EntityServices
{
    public interface ITasksService
    {
        Task<GeneralResponse> CreateAsync(TaskCreateDto dto, Guid userId);
        Task<GeneralResponse> GetByIdAsync(Guid taskId, Guid userId);
        Task<GeneralResponse> GetAllAsync(Guid userId,bool? completed,Priority? priority,Guid? categoryId,string? search,int page = 1,int pageSize = 10);
        Task<GeneralResponse> UpdateAsync(Guid taskId, TaskUpdateDto dto, Guid userId);
        Task<GeneralResponse> DeleteAsync(Guid taskId, Guid userId);
        Task<GeneralResponse> ToggleCompleteAsync(Guid taskId, Guid userId);
    }
}
