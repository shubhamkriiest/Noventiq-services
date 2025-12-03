using DotNetAssignment.Models;

namespace DotNetAssignment.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserWithRoleAsync(int id);
        Task<List<User>> GetAllUsersWithRolesAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
    }
}