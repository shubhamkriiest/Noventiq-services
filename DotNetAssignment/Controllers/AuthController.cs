using DotNetAssignment.DTOs;
using DotNetAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var (success, message, response) = await _authService.LoginAsync(loginDto);

            if (!success)
                return Unauthorized(new { message });

            return Ok(response);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto registerDto)
        {
            var (success, message) = await _authService.RegisterAsync(registerDto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}