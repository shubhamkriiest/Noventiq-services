using DotNetAssignment.DTOs;
using DotNetAssignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // Requires authentication for all endpoints
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SimpleLocalizer _localizer;

        public UsersController(IUserService userService, SimpleLocalizer localizer)
        {
            _userService = userService;
            _localizer = localizer;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
                return NotFound(new { message = _localizer.Get("UserNotFound", HttpContext) });

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createDto)
        {
            var (success, message, user) = await _userService.CreateUserAsync(createDto);

            if (!success)
                return BadRequest(new { message });

            return CreatedAtAction(nameof(GetUser), new { id = user!.Id }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            var (success, message) = await _userService.UpdateUserAsync(id, updateDto);

            if (!success)
            {
                // Check if it's a "not found" or validation error
                if (message == "User not found")
                    return NotFound(new { message = _localizer.Get("UserNotFound", HttpContext) });
                
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }
    }
}