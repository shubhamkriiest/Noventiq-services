using DotNetAssignment.Data;
using DotNetAssignment.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetAssignment.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _entities
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> HasUsersAsync(int roleId)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.RoleId == roleId);
        }
    }
}