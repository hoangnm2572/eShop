using BusinessObjects.DTOs;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string jwtKey, string jwtIssuer);
        Task ChangePasswordAsync(ChangePasswordRequestDTO request);
        Task ChangePasswordByBranchAsync(int branchId, string newPassword);
    }
}