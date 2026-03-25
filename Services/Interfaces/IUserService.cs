using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserResponseDTO> GetAllUsers();
        UserResponseDTO GetUserById(int id);
        void DeleteUser(int id);
        void ActivateUser(int id);
    }
}
