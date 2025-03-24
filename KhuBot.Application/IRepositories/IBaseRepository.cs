using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KhuBot.Application.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? conditionExpression = null,
            params Expression<Func<T, dynamic>>[] includeExpressions);

        Task<T?> FindAsync(Expression<Func<T, bool>>? conditionExpression = null,
            params Expression<Func<T, dynamic>>[] includeExpressions);

        Task<bool> IsExistsAsync(Expression<Func<T, bool>> conditionExpression);

        Task<T> CreateAsync(T model);

        Task UpdateAsync(T model);

        Task DeleteAsync(T model);
    }
}
