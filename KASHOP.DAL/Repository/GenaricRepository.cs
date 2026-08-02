using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    public class GenaricRepository<A> : IGenaricRepository<A> where A : class
    {
        private readonly ApplicationDbContext _context;

        public GenaricRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(A entity)
        {
            _context.Add(entity);
            var affected = await _context.SaveChangesAsync();
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(A entity)
        {
           _context.Remove(entity);
           var affected = await _context.SaveChangesAsync();
           return affected > 0;
        }

        public async Task<List<A>> GetAllAsync(string[]? includes = null)
        {
            IQueryable<A> query = _context.Set<A>();
            foreach(var include in includes)
            {
                if(include != null)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<A> GetOneAsync(Expression<Func<A, bool>> filter, string[]? includes = null)
        {
            IQueryable<A> query = _context.Set<A>();
            if(includes != null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(filter);

        }

        public async Task<bool> UpdateAsync(A entity)
        {
            _context.Update(entity);
            var affected = await _context.SaveChangesAsync();
            return affected > 0;
        }
    }
}

