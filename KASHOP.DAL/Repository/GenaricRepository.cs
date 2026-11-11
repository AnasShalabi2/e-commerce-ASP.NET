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
        public async Task<A> CreateAsync(A entity)
        {
            _context.Add(entity);
            return entity;
        }

        public void Delete(A entity)
        {
           _context.Remove(entity);
        }
        public IQueryable<A> getQueryable(Expression<Func<A, bool>>? filter = null, string[]? includes = null)
        {
            IQueryable<A> query = _context.Set<A>();
            if (includes != null)
            {
                foreach (var include in includes)
                {

                    query = query.Include(include);
                }
            }
            if (filter is not null)
                query = query.Where(filter);

            return query;
        }

        public async Task<List<A>> GetAllAsync(Expression<Func<A, bool>>? filter = null,string[]? includes = null)
        {
            IQueryable<A> query = _context.Set<A>();
            if (includes is not null)
            {
                foreach (var include in includes)
            {

                    query = query.Include(include);
                }
              
            }
            if (filter is not null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<A>? GetOneAsync(Expression<Func<A, bool>> ? filter = null, string[]? includes = null)
        {
            IQueryable<A> query = _context.Set<A>();
            if(includes is not null)
            {
                foreach(var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(filter);
        }

        public void Update(A entity)
        {
            _context.Update(entity);
        }
    }
}

