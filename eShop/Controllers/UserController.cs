using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(users);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/deactivate")]
        public IActionResult DeactivateUser(int id)
        {
            try
            {
                _userService.DeleteUser(id);
                return Ok(new { message = "Đã khóa tài khoản" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/activate")]
        public IActionResult ActivateUser(int id)
        {
            try
            {
                _userService.ActivateUser(id);
                return Ok(new { message = "Đã kích hoạt tài khoản" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequestDTO request)
        {
            try
            {
                _userService.UpdateUser(id, request);
                return Ok(new { message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
