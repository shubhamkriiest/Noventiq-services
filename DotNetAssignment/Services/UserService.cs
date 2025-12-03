using DotNetAssignment.DTOs;
using DotNetAssignment.Models;
using DotNetAssignment.Repositories;

namespace DotNetAssignment.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Role> _roleRepository;

        public UserService(IUserRepository userRepository, IRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersWithRolesAsync();
            
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.Role.Name,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithRoleAsync(id);
            
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleName = user.Role.Name,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<(bool Success, string Message, UserDto? User)> CreateUserAsync(CreateUserDto createDto)
        {
            // Business validation using repository
            if (await _userRepository.GetByUsernameAsync(createDto.Username) != null)
                return (false, "Username already exists", null);

            if (await _userRepository.GetByEmailAsync(createDto.Email) != null)
                return (false, "Email already exists", null);

            var roleExists = await _roleRepository.ExistsAsync(r => r.Id == createDto.RoleId);
            if (!roleExists)
                return (false, "Invalid role ID", null);

            // Business logic - create user with hashed password
            var user = new User
            {
                Username = createDto.Username,
                Email = createDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                RoleId = createDto.RoleId
            };

            await _userRepository.CreateAsync(user);

            // Reload with role
            user = await _userRepository.GetUserWithRoleAsync(user.Id);

            var userDto = new UserDto
            {
                Id = user!.Id,
                Username = user.Username,
                Email = user.Email,
                RoleName = user.Role.Name,
                CreatedAt = user.CreatedAt
            };

            return (true, "User created successfully", userDto);
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(int id, UpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, "User not found");

            // Check for conflicts
            var existingUsername = await _userRepository.GetByUsernameAsync(updateDto.Username);
            if (existingUsername != null && existingUsername.Id != id)
                return (false, "Username already exists");

            var existingEmail = await _userRepository.GetByEmailAsync(updateDto.Email);
            if (existingEmail != null && existingEmail.Id != id)
                return (false, "Email already exists");

            // Update properties
            user.Username = updateDto.Username;
            user.Email = updateDto.Email;
            user.RoleId = updateDto.RoleId;

            if (!string.IsNullOrEmpty(updateDto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);

            await _userRepository.UpdateAsync(user);

            return (true, "User updated successfully");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, "User not found");

            await _userRepository.RemoveAsync(user);

            return (true, "User deleted successfully");
        }
    }
}