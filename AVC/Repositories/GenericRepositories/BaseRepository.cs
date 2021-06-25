using AVC.Models;
using AVC.Dtos.PagingDtos;
using AVC.Extensions.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AVC.GenericRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public readonly DbContext _dbContext;
        public readonly DbSet<T> _dbSet;


        public BaseRepository(AVCContext dbContext) // 
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }


        public void Add(T t)
        {
            _dbSet.Add(t);
        }

        public void Delete(T t)
        {
            if (t != null)
            {
                _dbSet.Remove(t);
                return;
            }
            throw new ArgumentNullException(nameof(T));

        }

        public void Deactivate(T t)
        {
            if (t != null)
            {
                var property = t.GetType().GetProperty("IsAvailable");
                var propertyValue = (bool?)property.GetValue(t);
                if (!propertyValue.HasValue)
                {
                    // set the value
                    property.SetValue(t, false);
                }
                return;
            }
            throw new ArgumentNullException(nameof(T));

        }


        public T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            return DbSetIncluding(query, including).Where(predicate).FirstOrDefault<T>();
        }

        public T Get(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includer = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            if (includer != null)
                query = includer(query);

            return query.Where(predicate).FirstOrDefault<T>();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Update(T items)
        {
            //Default is nothing
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            return DbSetIncluding(query, including);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            return DbSetIncluding(query, including).Where(predicate);
        }

        public PagingDto<T> GetAll(int page, int limit, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including)
        {

            IQueryable<T> query = _dbSet.AsQueryable();

            PagingDto<T> dto = new PagingDto<T>(DbSetIncluding(query, including).Where(predicate), page, limit);

            return dto;
        }
        
        public PagingDto<T> GetAll(int page, int limit, Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includer = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (includer != null)
                query = includer(query);

            PagingDto<T> dto = new PagingDto<T>(query.Where(predicate), page, limit);

            return dto;
        }

        public PagingDto<T> GetAll(int page, int limit, params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            PagingDto<T> dto = new PagingDto<T>(DbSetIncluding(query, including), page, limit);

            return dto;
        }

        private IQueryable<T> DbSetIncluding(IQueryable<T> query, Expression<Func<T, object>>[] including)
        {
            if (including != null)
                including.ToList().ForEach(include =>
                {
                    if (include != null)
                        query = query.Include(include);
                });

            return query;
        }

        public PagingDto<T> GetAllWithOrdered(int page, int limit, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            PagingDto<T> dto = new PagingDto<T>(DbSetIncluding(query, including).OrderBy(orderBy), page, limit);

            return dto;
        }

        public PagingDto<T> GetAllWithOrdered(int page, int limit, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            PagingDto<T> dto = new PagingDto<T>(DbSetIncluding(query, including).Where(predicate).OrderBy(orderBy), page, limit);

            return dto;
        }

        public PagingDto<T> GetAllWithOrderedDecs(int page, int limit, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            PagingDto<T> dto = new PagingDto<T>(DbSetIncluding(query, including).OrderByDescending(orderBy), page, limit);

            return dto;
        }

        public PagingDto<T> GetAllWithOrderedDecs(int page, int limit, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            PagingDto<T> dto = new PagingDto<T>(DbSetIncluding(query, including).Where(predicate).OrderByDescending(orderBy), page, limit);

            return dto;
        }
    }
}
