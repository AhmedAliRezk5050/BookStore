using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T? GetFirstOrDefault(Expression<Func<T, bool>> filter);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string includedProperties = "");

        IEnumerable<T> GetAll();
        Task<List<T>> GetAllAsync(string includedProperties = "");

        void Add(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> range);
    }
}