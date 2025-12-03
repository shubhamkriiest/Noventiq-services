using DotNetAssignment.DTOs;

namespace DotNetAssignment.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginDto loginDto);
        Task<(bool Success, string Message)> RegisterAsync(CreateUserDto registerDto);
    }
}