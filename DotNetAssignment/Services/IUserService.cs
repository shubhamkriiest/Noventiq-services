using DotNetAssignment.DTOs;

namespace DotNetAssignment.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<(bool Success, string Message, UserDto? User)> CreateUserAsync(CreateUserDto createDto);
        Task<(bool Success, string Message)> UpdateUserAsync(int id, UpdateUserDto updateDto);
        Task<(bool Success, string Message)> DeleteUserAsync(int id);
    }
}