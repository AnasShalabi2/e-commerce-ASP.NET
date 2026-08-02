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
        Task<List<A>> GetAllAsync(string[]? includes = null);
        Task<bool> CreateAsync(A entity);
        Task<A> GetOneAsync(Expression<Func<A, bool>> filter, string[]? includes = null);
        Task<bool> DeleteAsync(A entity);
        Task <bool> UpdateAsync(A entity);


    }
}
