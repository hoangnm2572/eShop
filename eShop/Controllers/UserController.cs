using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { message = "Đã khóa tài khoản" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                await _userService.ActivateUserAsync(id);
                return Ok(new { message = "Đã kích hoạt tài khoản" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDTO request)
        {
            try
            {
                await _userService.UpdateUserAsync(id, request);
                return Ok(new { message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}