using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VibroDiagnostic.Core;
using VibroDiagnostic.Core.Interfaces;
using VibroDiagnostic.Data.Contexts;
using VibroDiagnostic.Data.Specification;

namespace VibroDiagnostic.Data.Repositories
{
    public class EfRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly SensorContext _dbContext;

        public EfRepository(SensorContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> GetFirstBySpecificationAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task AddBatchAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

        }


        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }

        public async Task UpdateBySpecificationAsync(ISpecification<T> spec, Action<SetPropertyBuilder<T>> setPropertyBuilder)
        {
            var query = ApplySpecification(spec);
            var builder = new SetPropertyBuilder<T>();
            setPropertyBuilder.Invoke(builder);
            await query.ExecuteUpdateAsync(builder.SetPropertyCalls);
            await _dbContext.SaveChangesAsync();
        }
    }
}
