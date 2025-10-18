using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.AcademicManagement;

public interface IEducationRepository : IGenericRepository<Education>
{
    Task<ActionResponse<IEnumerable<Education>>> GetByInstitutionIdAsync(Guid institutionId);
    Task<ActionResponse<Education>> GetByIdWithInstitutionAsync(Guid id);
}
