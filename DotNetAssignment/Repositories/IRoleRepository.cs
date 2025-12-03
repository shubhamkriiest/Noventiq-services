using DotNetAssignment.Models;

namespace DotNetAssignment.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string name);
        Task<bool> HasUsersAsync(int roleId);
    }
}