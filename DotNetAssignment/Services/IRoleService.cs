using DotNetAssignment.DTOs;

namespace DotNetAssignment.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<(bool Success, string Message, RoleDto? Role)> CreateRoleAsync(CreateRoleDto createDto);
        Task<(bool Success, string Message)> UpdateRoleAsync(int id, UpdateRoleDto updateDto);
        Task<(bool Success, string Message)> DeleteRoleAsync(int id);
    }
}