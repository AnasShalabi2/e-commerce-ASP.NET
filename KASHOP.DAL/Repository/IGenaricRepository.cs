using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    public interface IGenaricRepository<A> where A : class
    {
        Task<List<A>> GetAllAsync(Expression<Func<A, bool>> ? filter = null, string[]? includes = null);
        Task<A> CreateAsync(A entity);
        Task<A>? GetOneAsync(Expression<Func<A, bool>> filter, string[]? includes = null);
        void Delete(A entity);
        void Update(A entity);
        public IQueryable<A> getQueryable(Expression<Func<A, bool>>? filter = null, string[]? includes = null);


    }
}
