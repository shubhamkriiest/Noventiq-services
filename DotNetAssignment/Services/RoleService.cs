using DotNetAssignment.DTOs;
using DotNetAssignment.Models;
using DotNetAssignment.Repositories;

namespace DotNetAssignment.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }

        public async Task<(bool Success, string Message, RoleDto? Role)> CreateRoleAsync(CreateRoleDto createDto)
        {
            // Business validation - check if role name already exists
            var existingRole = await _roleRepository.GetByNameAsync(createDto.Name);
            if (existingRole != null)
                return (false, "Role name already exists", null);

            // Create new role
            var role = new Role
            {
                Name = createDto.Name,
                Description = createDto.Description
            };

            await _roleRepository.CreateAsync(role);

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            return (true, "Role created successfully", roleDto);
        }

        public async Task<(bool Success, string Message)> UpdateRoleAsync(int id, UpdateRoleDto updateDto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return (false, "Role not found");

            // Check if new name conflicts with another role
            var existingRole = await _roleRepository.GetByNameAsync(updateDto.Name);
            if (existingRole != null && existingRole.Id != id)
                return (false, "Role name already exists");

            // Update properties
            role.Name = updateDto.Name;
            role.Description = updateDto.Description;

            await _roleRepository.UpdateAsync(role);

            return (true, "Role updated successfully");
        }

        public async Task<(bool Success, string Message)> DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return (false, "Role not found");

            // Business rule: Can't delete role if users are assigned to it
            var hasUsers = await _roleRepository.HasUsersAsync(id);
            if (hasUsers)
                return (false, "Cannot delete role - users are assigned to this role");

            await _roleRepository.RemoveAsync(role);

            return (true, "Role deleted successfully");
        }
    }
}