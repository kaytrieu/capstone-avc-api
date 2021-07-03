using AVC.Dtos.PagingDtos;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AVC.GenericRepository
{
    public interface IBaseRepository<T> where T : class
    {

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including);
        PagingDto<T> GetAll(int page, int limit, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including);
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] including);
        PagingDto<T> GetAll(int page, int limit, params Expression<Func<T, object>>[] including);
        public PagingDto<T> GetAll(int page, int limit, Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includer = null);
        public PagingDto<T> GetAll(int page, int limit, Func<IQueryable<T>, IIncludableQueryable<T, object>> includer = null);
        PagingDto<T> GetAllWithOrdered(int page, int limit, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including);
        PagingDto<T> GetAllWithOrdered(int page, int limit, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including);
        PagingDto<T> GetAllWithOrdered(int page, int limit, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, Func<IQueryable<T>, IIncludableQueryable<T, object>> includer = null);
        PagingDto<T> GetAllWithOrderedDecs(int page, int limit, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including);
        PagingDto<T> GetAllWithOrderedDecs(int page, int limit, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, params Expression<Func<T, object>>[] including);
        PagingDto<T> GetAllWithOrderedDecs(int page, int limit, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, Func<IQueryable<T>, IIncludableQueryable<T, object>> includer = null);
        T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] including);
        T Get(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includer = null);
        void Add(T t);
        void Delete(T t);
        void Deactivate(T t);
        void Update(T items);
        int SaveChanges();
        Task<int> SaveChangesAsync();

    }
}
