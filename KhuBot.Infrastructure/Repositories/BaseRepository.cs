using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KhuBot.Application.IRepositories;
using KhuBot.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KhuBot.Infrastructure.Repositories
{
    public class BaseRepository<T>(ApplicationDbContext dbContext) : IBaseRepository<T> where T : class
    {
        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? conditionExpression = null,
            params Expression<Func<T, dynamic>>[] includeExpressions)
        {
            var query = dbContext.Set<T>()
                .AsNoTracking();
            if (conditionExpression != null)
                query = query.Where(conditionExpression);
            if (includeExpressions.Length != 0)
                query = includeExpressions.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));

            return await query.ToListAsync();
        }

        public virtual async Task<T?> FindAsync(Expression<Func<T, bool>>? conditionExpression = null,
            params Expression<Func<T, dynamic>>[] includeExpressions)
        {
            var query = dbContext.Set<T>()
                .AsNoTracking();
            if (conditionExpression != null)
                query = query.Where(conditionExpression);
            if (includeExpressions.Length != 0)
                query = includeExpressions.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<bool> IsExistsAsync(Expression<Func<T, bool>> conditionExpression)
        {
            return await dbContext.Set<T>().AnyAsync(conditionExpression);
        }

        public virtual async Task<T> CreateAsync(T model)
        {
            await dbContext.Set<T>().AddAsync(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public virtual async Task UpdateAsync(T model)
        {
            dbContext.Set<T>().Update(model);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T model)
        {
            dbContext.Set<T>().Remove(model);
            await dbContext.SaveChangesAsync();
        }
    }
}
