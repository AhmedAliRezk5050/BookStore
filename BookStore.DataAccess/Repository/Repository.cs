using BookStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _context;

        internal DbSet<T> dbSet;
        
        public Repository(DataContext context)
        {
            _context = context;

            dbSet = _context.Set<T>();
        }


        public void Add(T entity)
        {
            dbSet.Add(entity);
        }


        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;

            return query.ToList();
        }

        public Task<List<T>> GetAllAsync()
        {
            IQueryable<T> query = dbSet;
            return query.ToListAsync();
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet.Where(filter);

            return query.FirstOrDefault();
        }

        public Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet.Where(filter);

            return query.FirstOrDefaultAsync();
        }


        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> range)
        {
            dbSet.RemoveRange(range);
        }
    }
}