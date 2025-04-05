using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories_Interfaces;
using Talabat.Core.Specification;
using Talabat.Repository.Data;


namespace Talabat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IEnumerable<T>)await _dbContext.Set<Product>()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToListAsync();
            }
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                var product = await _dbContext.Set<Product>()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);
                return product as T;
            }
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("No Entity With The Given Id Was Found...!");

            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllWithSpec(ISpecification<T> Spec)
        {
            return await Specification_Evaluator<T>.GetQuery( _dbContext.Set<T>(),Spec).ToListAsync();
            
        }

        public async Task<T> GetWithSpec(ISpecification<T> Spec)
        {
            return await Specification_Evaluator<T>.GetQuery(_dbContext.Set<T>(),Spec).FirstOrDefaultAsync();
        }
    }
}