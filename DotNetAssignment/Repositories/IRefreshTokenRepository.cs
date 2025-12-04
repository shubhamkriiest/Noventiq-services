using DotNetAssignment.Models;

namespace DotNetAssignment.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task CreateAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
    }
}
