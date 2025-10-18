using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.WorkManagement;

public interface IWorkFieldRepository : IGenericRepository<WorkField>
{
    Task<ActionResponse<IEnumerable<WorkField>>> GetByCategoryIdAsync(Guid categoryId);
    Task<ActionResponse<WorkField>> GetByIdWithCategoryAsync(Guid id);
}
