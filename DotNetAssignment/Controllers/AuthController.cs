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
        private readonly SimpleLocalizer _localizer;

        public AuthController(IAuthService authService, SimpleLocalizer localizer)
        {
            _authService = authService;
            _localizer = localizer;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var (success, message, response) = await _authService.LoginAsync(loginDto);

            if (!success)
                return Unauthorized(new { message = _localizer.Get(message ?? "InvalidCredentials", HttpContext) });

            return Ok(response);
        }
        
        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            var result = await _authService.RefreshAsync(request.RefreshToken);
            if (!result.Success)
                return BadRequest(new { message = _localizer.Get(result.Message ?? "RefreshFailed", HttpContext) });
            return Ok(result.Response);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto registerDto)
        {
            var (success, message) = await _authService.RegisterAsync(registerDto);

            if (!success)
                return BadRequest(new { message = _localizer.Get(message ?? "RegisterFailed", HttpContext) });

            return Ok(new { message = _localizer.Get(message ?? "RegistrationSuccessful", HttpContext) });
        }
    }
}