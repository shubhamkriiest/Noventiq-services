using DotNetAssignment.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DotNetAssignment.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _entities;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _entities = _dbContext.Set<T>();
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public virtual async Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.AnyAsync(predicate);
        }

        public virtual async Task<List<T>> FindManyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await _entities.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task RemoveAsync(T entity)
        {
            _entities.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}