using DotNetAssignment.DTOs;
using DotNetAssignment.Models;
using DotNetAssignment.Repositories;
using Microsoft.AspNetCore.Http;
using DotNetAssignment.Services;

namespace DotNetAssignment.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly SimpleLocalizer _simpleLocalizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleService(IRoleRepository roleRepository, SimpleLocalizer simpleLocalizer, IHttpContextAccessor httpContextAccessor)
        {
            _roleRepository = roleRepository;
            _simpleLocalizer = simpleLocalizer;
            _httpContextAccessor = httpContextAccessor;
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
                return (false, _simpleLocalizer.Get("RoleNameExists", _httpContextAccessor.HttpContext), null);

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

            return (true, _simpleLocalizer.Get("RoleCreated", _httpContextAccessor.HttpContext), roleDto);
        }

        public async Task<(bool Success, string Message)> UpdateRoleAsync(int id, UpdateRoleDto updateDto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return (false, _simpleLocalizer.Get("RoleNotFound", _httpContextAccessor.HttpContext));

            // Check if new name conflicts with another role
            var existingRole = await _roleRepository.GetByNameAsync(updateDto.Name);
            if (existingRole != null && existingRole.Id != id)
                return (false, _simpleLocalizer.Get("RoleNameExists", _httpContextAccessor.HttpContext));

            // Update properties
            role.Name = updateDto.Name;
            role.Description = updateDto.Description;

            await _roleRepository.UpdateAsync(role);

            return (true, _simpleLocalizer.Get("RoleUpdated", _httpContextAccessor.HttpContext));
        }

        public async Task<(bool Success, string Message)> DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return (false, _simpleLocalizer.Get("RoleNotFound", _httpContextAccessor.HttpContext));

            // Business rule: Can't delete role if users are assigned to it
            var hasUsers = await _roleRepository.HasUsersAsync(id);
            if (hasUsers)
                return (false, _simpleLocalizer.Get("RoleHasUsers", _httpContextAccessor.HttpContext));

            await _roleRepository.RemoveAsync(role);

            return (true, _simpleLocalizer.Get("RoleDeleted", _httpContextAccessor.HttpContext));
        }
    }
}