using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
        public IActionResult Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                _authService.Register(request);
                return Ok(new { message = "Tạo tài khoản thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                string jwtKey = _configuration["JwtSettings:Key"] ?? throw new Exception("Missing JWT Key");
                string jwtIssuer = _configuration["JwtSettings:Issuer"] ?? throw new Exception("Missing JWT Issuer");

                var response = _authService.Login(request, jwtKey, jwtIssuer);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            try
            {
                _authService.ChangePassword(request);
                return Ok(new { message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("change-password-by-branch/{branchId}")]
        public IActionResult ChangePasswordByBranch(int branchId, [FromBody] ChangePasswordRequestDTO request)
        {
            try
            {
                _authService.ChangePasswordByBranch(branchId, request.NewPassword);
                return Ok(new { message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
