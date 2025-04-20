using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibroDiagnostic.Core.Interfaces;

namespace VibroDiagnostic.Data
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        
        Task<T> GetFirstBySpecificationAsync(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task UpdateBySpecificationAsync(ISpecification<T> spec, Action<SetPropertyBuilder<T>> setPropertyBuilder);
        Task DeleteAsync(T entity);
        Task AddBatchAsync(IEnumerable<T> entities);
        Task<int> CountAsync(ISpecification<T> spec);
    }
}
