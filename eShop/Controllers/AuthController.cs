using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                await _authService.RegisterAsync(request);
                return Ok(new { message = "Tạo tài khoản thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                string jwtKey = _configuration["JwtSettings:Key"] ?? throw new Exception("Missing JWT Key");
                string jwtIssuer = _configuration["JwtSettings:Issuer"] ?? throw new Exception("Missing JWT Issuer");

                var response = await _authService.LoginAsync(request, jwtKey, jwtIssuer);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            try
            {
                var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userIdFromToken != request.TargetUserId)
                    return Forbid("Bạn không có quyền đổi mật khẩu của tài khoản khác!");

                await _authService.ChangePasswordAsync(request);
                return Ok(new { message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("change-password-by-branch/{branchId}")]
        public async Task<IActionResult> ChangePasswordByBranch(int branchId, [FromBody] ChangePasswordRequestDTO request)
        {
            try
            {
                await _authService.ChangePasswordByBranchAsync(branchId, request.NewPassword);
                return Ok(new { message = "Đổi mật khẩu chi nhánh thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}