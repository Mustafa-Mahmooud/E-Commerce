using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specification;

namespace Talabat.Core.Repositories_Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task <IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> GetAllWithSpec( ISpecification<T> Spec);
        Task<T> GetWithSpec(ISpecification<T> Spec);

        Task DeleteAsync(int id);




    }
}
