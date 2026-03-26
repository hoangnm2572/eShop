using BusinessObjects.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO?> GetUserByIdAsync(int id);
        Task DeleteUserAsync(int id);
        Task ActivateUserAsync(int id);
        Task UpdateUserAsync(int userId, UpdateUserRequestDTO request);
    }
}