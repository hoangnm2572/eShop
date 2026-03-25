using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        void Register(RegisterRequestDTO request);
        AuthResponseDTO Login(LoginRequestDTO request, string jwtKey, string jwtIssuer);
        void ChangePassword(ChangePasswordRequestDTO request);
        void ChangePasswordByBranch(int branchId, string newPassword);
    }
}
