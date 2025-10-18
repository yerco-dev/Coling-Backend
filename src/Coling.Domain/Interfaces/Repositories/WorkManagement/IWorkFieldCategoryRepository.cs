using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.WorkManagement;

public interface IWorkFieldCategoryRepository : IGenericRepository<WorkFieldCategory>
{
    Task<ActionResponse<IEnumerable<WorkFieldCategory>>> GetAllWithWorkFieldsAsync();
    Task<ActionResponse<WorkFieldCategory>> GetByIdWithWorkFieldsAsync(Guid id);
}
