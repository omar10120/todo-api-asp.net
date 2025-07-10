using ayagroup_SMS.Core.DTOs.Requestes;
using ayagroup_SMS.Core.DTOs.Responses;
namespace ayagroup_SMS.Core.Interfaces.Application.EntityServices
{
    public interface IUserService
    {
        Task<GeneralResponse> ProfileAsync(Guid userid);
        Task<GeneralResponse> GetByIdAsync(Guid id);
        
        Task<GeneralResponse> RegisterAsync(RegisterRequestDto user);
        Task<GeneralResponse> LoginAsync(LoginRequestDto user);
        Task<GeneralResponse> GetAllUsersAsync();
        
    }
}
