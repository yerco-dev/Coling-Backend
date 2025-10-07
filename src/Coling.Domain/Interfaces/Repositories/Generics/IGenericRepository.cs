using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Interfaces.BaseEntities;
using System.Linq.Expressions;

namespace Coling.Domain.Interfaces.Repositories.Generics;

public interface IGenericRepository<T> where T : IBaseEntity
{
    Task<ActionResponse<T>> GetAsync(Guid id, bool includeDeleteds = true);
    Task<ActionResponse<IEnumerable<T>>> GetAsync(bool includeDeleteds = true);
    Task<ActionResponse<T>> GetAsync(Expression<Func<T, bool>> predicate);
    Task<ActionResponse<T>> AddAsync(T entity);
    Task<ActionResponse<T>> UpdateAsync(T entity);
    Task<ActionResponse<T>> DeleteAsync(Guid id);
    Task<ActionResponse<T>> RestoreRegisterAsync(Guid id);
}