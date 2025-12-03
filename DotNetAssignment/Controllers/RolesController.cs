using DotNetAssignment.DTOs;
using DotNetAssignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Requires authentication for all endpoints
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        // GET: api/roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
                return NotFound(new { message = "Role not found" });

            return Ok(role);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createDto)
        {
            var (success, message, role) = await _roleService.CreateRoleAsync(createDto);

            if (!success)
                return BadRequest(new { message });

            return CreatedAtAction(nameof(GetRole), new { id = role!.Id }, role);
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto updateDto)
        {
            var (success, message) = await _roleService.UpdateRoleAsync(id, updateDto);

            if (!success)
            {
                if (message == "Role not found")
                    return NotFound(new { message });

                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var (success, message) = await _roleService.DeleteRoleAsync(id);

            if (!success)
            {
                if (message == "Role not found")
                    return NotFound(new { message });

                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}