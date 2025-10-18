using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.InstitutionManagement;

public interface IInstitutionRepository : IGenericRepository<Institution>
{
    Task<ActionResponse<IEnumerable<Institution>>> GetByInstitutionTypeIdAsync(Guid institutionTypeId);
    Task<ActionResponse<Institution>> GetByIdWithTypeAsync(Guid id);
    Task<ActionResponse<IEnumerable<Institution>>> GetByNameAsync(string name);
}
