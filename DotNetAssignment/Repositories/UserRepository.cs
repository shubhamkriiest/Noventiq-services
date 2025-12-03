using DotNetAssignment.Data;
using DotNetAssignment.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetAssignment.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User?> GetUserWithRoleAsync(int id)
        {
            return await _entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllUsersWithRolesAsync()
        {
            return await _entities
                .Include(u => u.Role)
                .ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _entities
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _entities
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}