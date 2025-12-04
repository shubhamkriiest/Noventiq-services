using DotNetAssignment.Data;
using DotNetAssignment.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetAssignment.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _db;
        public RefreshTokenRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<RefreshToken?> GetByTokenAsync(string token)
            => _db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(r => r.Token == token);

        public async Task CreateAsync(RefreshToken refreshToken)
        {
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _db.RefreshTokens.Update(refreshToken);
            await _db.SaveChangesAsync();
        }
    }
}
